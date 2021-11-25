﻿namespace MapNotepad.AppConstants
{
    public static class Constants
    {
        public static string AppName = "MapNotepad";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "Q0mxMu0s1HmcJt7hVVF1thg2xfEBRyuIXfIVIw4JVImdvKNxjP";
        public static string AndroidClientId = "124598732874-4t34avmo2oh78qmu76tnkoeh8g489kj4.apps.googleusercontent.com";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://oauth2.googleapis.com/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "Q0mxMu0s1HmcJt7hVVF1thg2xfEBRyuIXfIVIw4JVImdvKNxjP:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.124598732874-4t34avmo2oh78qmu76tnkoeh8g489kj4:/oauth2redirect";
    }
}
