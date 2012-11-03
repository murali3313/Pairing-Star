  $(document).ready(function () {
  $("#saveChanges").click(SaveChanges);
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
					
			
        });

        function SaveChanges() {
            localStorage["PSUrl"] = $("#txtUrl").val();
            localStorage["UserName"] = $("#txtUserName").val();			
        }
