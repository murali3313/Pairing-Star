using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PairingStar.Models;

namespace PairingStar.Controllers
{
    public class ManageProjectController : Controller
    {

        #region View Users
        public ActionResult ViewUsers()
        {
           ViewData["Users"]=GetAllUserDetails();
            return View();
        }

        public string DeleteUserData(string  userName)
        {
            Repository.GetRepository().ExecuteQuery("Delete from t_user where UserName='"+userName+"'");
            Repository.GetRepository().ExecuteQuery("Delete from t_pairingmatrix where pairone='" + userName + "' OR pairtwo='" + userName+ "'");
            return userName;
        }

        private List<UserModel> GetAllUserDetails()
        {
            var dataTable = Repository.GetRepository().LoadData("Select * from t_user");
            var userModels = new List<UserModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                var userModel = new UserModel();
                userModel.UserName= row["UserName"] as string;
                userModel.Role= row["Role"] as string;
                userModel.Gender= row["Gender"] as string;
                userModel.Photo= row["Photo"] as Byte[];
                userModel.ID = Convert.ToInt32(row["PK_ID"]); 
                userModels.Add(userModel);
            }

            return userModels;
        }

        public ActionResult Show(int id)
        {

            var dataTable = Repository.GetRepository().LoadData("Select * from t_user where PK_ID=" + id);
            var imageData = dataTable.Rows[0]["Photo"] as byte[];
            var gender = dataTable.Rows[0]["Gender"] as string;
            
            var dir = Server.MapPath("~\\Images");
            if (imageData == null)
                if (gender == "Male")
                {
                    var path = Path.Combine(dir, "male-silhouette.jpg");
                    return File(path, "image/jpg");
                }
                else
                {
                    var path = Path.Combine(dir, "female-silhouette.jpg");
                    return File(path, "image/jpg");
                }

            return File(imageData, "image/jpg");
        }
        #endregion View Users

        #region Add User
        public ActionResult AddUser()
        {
            ViewData["Roles"] = new List<string>(Enum.GetNames(typeof(Role))).AsEnumerable();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddUser(UserModel userModel,HttpPostedFileBase uploadFile)
        {
            ViewData["Roles"] = new List<string>(Enum.GetNames(typeof(Role))).AsEnumerable();
         if(ModelState.IsValid)
         {
             if (uploadFile != null)
             {
                 userModel.Photo = new byte[uploadFile.ContentLength];
                 uploadFile.InputStream.Read(userModel.Photo, 0, uploadFile.ContentLength);
             }
             var repository = Repository.GetRepository();
             var isUserAlreadyExist = repository.ExecuteScalar<int>("Select count(*) From t_user where UserName='"+userModel.UserName+"'");
             if(isUserAlreadyExist>0)
                 ModelState.AddModelError("UserAlreadyExists","User already exists.please enter a new user.");
             else
             {
                 var sqLiteParameters = new[]
                                            {
                                                new SQLiteParameter("@userName", DbType.String, 200)
                                                    {Value = userModel.UserName},
                                                new SQLiteParameter("@role", DbType.String, 20) {Value = userModel.Role}
                                                ,
                                                new SQLiteParameter("@photo", DbType.Binary, 20)
                                                    {Value = userModel.Photo},
                                                new SQLiteParameter("@gender", DbType.String, 20)
                                                    {Value = userModel.Gender}
                                            };

                 repository.ExecuteQuery("insert into t_user values(null,@userName,@role,@photo,@gender);",
                                         sqLiteParameters);

                 return RedirectToAction("ViewUsers");
             }
         }
            return View();
        }
        #endregion Add User

        #region Update Pair Star
        public ActionResult UpdatePairStar(string pairName)
        {
            ViewBag.UserInfoAvailable = !string.IsNullOrEmpty(pairName);
            var pairModel=new UpdatePairModel();
            if (!ViewBag.UserInfoAvailable)
            {
                var dataTable = Repository.GetRepository().LoadData("Select UserName from t_user");
                var usernames = (from DataRow row in dataTable.Rows select row["UserName"] as string).ToList();
                usernames.Insert(0,"Please select one");
                ViewBag.UserNames = usernames;
            }
            else
            {
                var allUserDetails = GetAllUserDetails();
                var pairOne = allUserDetails.FirstOrDefault(model => model.UserName.ToUpper() == pairName.ToUpper());
                if (pairOne == null)
                    ViewBag.UserInfoAvailable = false;
                pairModel.PairOne = pairOne;
                pairModel.OtherUsers= allUserDetails.Except(new List<UserModel>() {pairOne});
            }
            return View(pairModel);
        }

        public string UpdatePairStarData(string pairName, string secondPair, string date,string time,string overrideData)
        {
            
            var timeWorked = Convert.ToDouble(time);
            var toOverride = Convert.ToBoolean(overrideData);
            if(toOverride)
            {
                UpdatePairInfo(pairName, secondPair, date, time);
                return "success";
            }

            var dataAlreadyAvail = Repository.GetRepository().LoadData(@"Select count(*) as PairCount,sum(pairtime) as TotalPairTime from t_pairingmatrix 
                                                                where ((pairone='" + pairName + "' and pairtwo='" + secondPair + "') OR"+
                                                                              " (pairone='" + secondPair+ "' and pairtwo='" + pairName+ "')) AND pairdate ='"+date+"'");
            var pairCount = Convert.ToInt32(dataAlreadyAvail.Rows[0]["PairCount"]);
            var totalPairTime = dataAlreadyAvail.Rows[0]["TotalPairTime"] == DBNull.Value ? 0 : Convert.ToDouble(dataAlreadyAvail.Rows[0]["TotalPairTime"]);
            if (pairCount > 0)
                return string.Format("The pair {0} and {1} have paired on {2} for {3} day.",pairName,secondPair,date,totalPairTime);

            var pairOneExceededPairTime = Repository.GetRepository().ExecuteScalar<double>("Select Coalesce(sum(pairtime),0) from t_pairingmatrix where (pairone='" +
                                                                              pairName + "' OR pairtwo='" + pairName + "') AND pairdate='" +
                                                                              date + "'");
            if (pairOneExceededPairTime +timeWorked > 1.0)
                return string.Format("The pair {0} has logged {1} day on this date already.", pairName,
                                     pairOneExceededPairTime);

            var pairTwoExceededPairTime = Repository.GetRepository().ExecuteScalar<double>("Select Coalesce(sum(pairtime),0) from t_pairingmatrix where (pairone='" +
                                                                              secondPair + "' OR pairtwo='" + secondPair+ "') AND pairdate='" +
                                                                              date + "'");
            if (pairTwoExceededPairTime+timeWorked > 1.0)
                return string.Format("The pair {0} has logged {1} day on this date already.", secondPair,
                                     pairTwoExceededPairTime);





            UpdatePairInfo(pairName, secondPair, date, time);
            return "success";
        }

        private void UpdatePairInfo(string pairName, string secondPair, string date, string time)
        {
            double pairTime;
            double.TryParse(time, out pairTime);
            DeleteDataIfPairtimeExceedsOneDay(pairName, date, pairTime);
            DeleteDataIfPairtimeExceedsOneDay(secondPair, date, pairTime);

            var dataAlreadyAvail = Repository.GetRepository().ExecuteScalar<int>(@"Select count(*) as PairCount from t_pairingmatrix 
                                                                where ((pairone='" + pairName + "' and pairtwo='" + secondPair + "') OR" +
                                                                              " (pairone='" + secondPair + "' and pairtwo='" + pairName + "')) AND pairdate ='" + date + "'");
            if (dataAlreadyAvail>0)
            {
                Repository.GetRepository().ExecuteQuery(@"update t_pairingmatrix set pairtime='" + time + "' "+  
                                                                "where ((pairone='" + pairName + "' and pairtwo='" + secondPair + "') OR" +
                                                                              " (pairone='" + secondPair + "' and pairtwo='" + pairName + "')) AND pairdate ='" + date + "'");
            }
            else
            {
                Repository.GetRepository().ExecuteQuery(string.Format("insert into t_pairingmatrix values(null,'{0}','{1}','{2}',{3})",pairName,secondPair,date,time));
            }
        }

        private void DeleteDataIfPairtimeExceedsOneDay(string pairName, string date, double pairTime)
        {
            var onePairTime = Repository.GetRepository().ExecuteScalar<double>("Select Coalesce(sum(pairtime),0) from t_pairingmatrix where (pairone='" +
                                                                                   pairName + "' OR pairtwo='" + pairName + "') AND pairdate='" +
                                                                                   date + "'");

            if(onePairTime+pairTime>1.0)
            {
                Repository.GetRepository().ExecuteQuery("Delete from t_pairingmatrix where (pairone='"+pairName+"' OR pairtwo='"+pairName+"') AND pairdate='"+date+"'");
            }
        }

        #endregion Update pair Star


        #region View Statistics
        public ActionResult ViewStatistics()
        {
            ViewBag.PairingDetails= Repository.GetRepository().LoadData("Select * from t_pairingmatrix").Rows.Cast<DataRow>()   ;

            return View();
        }
        #endregion


        #region Update Pair Extn
        public ActionResult UpdatePairStarExtn(string pairName)
        {
            
            if (string.IsNullOrEmpty(pairName))
            {
                return RedirectToAction("UpdatepairStar");
            }
            var allUserDetails = GetAllUserDetails();
            var pairOne = allUserDetails.FirstOrDefault(model => model.UserName.ToUpper() == pairName.ToUpper());
            if (pairOne == null)
                return RedirectToAction("UpdatepairStar");

            var pairModel = new UpdatePairModel
                                {
                                    PairOne = pairOne,
                                    OtherUsers = allUserDetails.Except(new List<UserModel>() {pairOne})
                                };

            return View(pairModel);
        }

        #endregion

    }
}
