<%@ Application Language="C#" %>

<script runat="server">

    static System.Timers.Timer _timer = null;
    
    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        log4net.Config.XmlConfigurator.Configure();
        if (_timer == null)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 24*60*60*1000; //interval set to 24 hours (miliseconds interval)
            _timer.Elapsed += GetDriversForTomorrow; //delegate

            // Have the timer fire repeated events (true is the default)
            _timer.AutoReset = true;
            // Start the timer
            _timer.Enabled = true;

            // If the timer is declared in a long-running method, use KeepAlive to prevent garbage collection
            GC.KeepAlive(_timer);
        }

    }
    static void GetDriversForTomorrow(Object source, System.Timers.ElapsedEventArgs e)
    {
        Volunteer v = new Volunteer();
        RidePat r = new RidePat();
        List<Volunteer> volunteersForPush = new List<Volunteer>();
        List<int> ridesId = new List<int>();
        volunteersForPush = r.GetRidePatViewForTomorrow(ref ridesId);
        Message m = new Message();
        for (int i = 0; i < volunteersForPush.Count; i++)
        {
            //push notification to each driver that has a ride tomorrow.
            m.rideIsTomorrow(ridesId[i], volunteersForPush[i]);
        }

    }
    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
