var url;
	    $(document).ready(function () {
		 $("#PSNotConfigured").attr("style", "display:block");
		 $("#openSettings").click(openSettingsPage);
			$("#openWebsite").click(openWebsite);
		 url = localStorage["PSUrl"];
	        var userName = localStorage["UserName"];

	        if (url == undefined || userName == undefined || url == "" || userName == "") {
	            $("#PSNotConfigured").attr("style", "display:block");
	            $("#PairingStarWindow").attr("style", "display:none");
	            return;
	        }
	        var completeURL = url + "/ManageProject/UpdatepairStarExtn?pairName=" + userName;
			
	        $("#PairingStarWindow").attr("src", completeURL);			
			
	    });
		

		
var openSettingsPage= function()
{
chrome.tabs.create({url: '/options.html'});
};

var openWebsite = function()
{
chrome.tabs.create({url: url+"/ManageProject/ViewStatistics"});
};