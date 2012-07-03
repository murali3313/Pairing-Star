var notificationInterval;
initTimer= function()
{
if(localStorage["IsNotifyEnabled"]=='true')
	{
	localStorage["IsNotifyShown"]=0;
	notificationInterval=setInterval(notifyUser(),1000);
	}
}
notifyUser= function()
{	
	var time=new Date();
	var currentHour=time.getHours();
	var currentMin=time.getMinutes();
	var notifyHour=localStorage["NotificationHour"];
	
	if((notifyHour==currentHour && currentMin==0)||
				(currentHour-notifyHour<=4 && localStorage["IsNotifyShown"]==0))
		{
			localStorage["IsNotifyShown"]=1;
			var userName=localStorage["UserName"];
			var notification = webkitNotifications.createHTMLNotification('notification.html');			
			notification.show();
		}
		
}
stopNotification=function()
{
	localStorage["IsNotifyShown"]=0;
	clearInterval(notificationInterval);
}
initTimer();