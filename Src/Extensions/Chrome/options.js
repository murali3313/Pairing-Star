  $(document).ready(function () {
            var PSUrl = localStorage["PSUrl"];
            if (PSUrl != undefined) {
                $("#txtUrl").val(PSUrl);
            }
            var UserName = localStorage["UserName"];
            if (PSUrl != undefined) {
                $("#txtUserName").val(UserName);
            }
			var notificationTime = localStorage["NotificationTime"];
            if (notificationTime != undefined) {
                $("#cmbNotification").val(notificationTime);
            }
			var AMPM = localStorage["AMPM"];
            if (AMPM != undefined) {
                $("#cmbAMPM").val(AMPM);
            }		
			
			var IsNotifyEnabled = localStorage["IsNotifyEnabled"];
            if (IsNotifyEnabled != undefined) {
                $("#chkSwitchOffNotification").attr('checked',IsNotifyEnabled=='true'?true:false);
            }		
			
			$("#saveChanges").click(SaveChanges);
			
        });

        function SaveChanges() {
            localStorage["PSUrl"] = $("#txtUrl").val();
            localStorage["UserName"] = $("#txtUserName").val();
			localStorage["IsNotifyEnabled"] = $("#chkSwitchOffNotification").attr('checked');
			localStorage["NotificationTime"] = $("#cmbNotification").val();
			localStorage["AMPM"] = $("#cmbAMPM").val();
			if(localStorage["AMPM"]=='AM')
			{
			  localStorage["NotificationHour"] = localStorage["NotificationTime"];
			}
			else
			{
			  localStorage["NotificationHour"]=localStorage["NotificationTime"]+12==24?0:12+localStorage["NotificationTime"];			 
			}
			chrome.extension.getBackgroundPage().initTimer();
        }
