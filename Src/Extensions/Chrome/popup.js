var url;
	    $(document).ready(function () {
		url = localStorage["PSUrl"];
	        var userName = localStorage["UserName"];

	        if (url == undefined || userName == undefined || url == "" || userName == "") {
	            $("#PSNotConfigured").attr("style", "display:block");
	            $("#PairingStarWindow").attr("style", "display:none");
	            return;
	        }
	        var completeURL = url + "/ManageProject/UpdatepairStarExtn?pairName=" + userName;
			
	        $("#PairingStarWindow").attr("src", completeURL);
			
			$("#openSettings").click(openSettingsPage);
			$("#openWebsite").click(openWebsite);
	    });
		

		
var openSettingsPage= function()
{
chrome.tabs.create({url: '/options.html'});
};

var openWebsite = function()
{
chrome.tabs.create({url: url+"/ManageProject/ViewStatistics"});
};