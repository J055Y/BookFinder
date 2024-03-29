﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Database;

namespace FunkyApp.Droid
{
    public static class AppDataHelper
    {
        public static FirebaseDatabase GetDatabase()
        {
            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId("ml-vision-f0b65")
                    .SetApiKey("AIzaSyDo9b6JcZGBkGEi4V0xeJKmkWxAwPDO6co")
                    .SetDatabaseUrl("https://ml-vision-f0b65.firebaseio.com")
                    .SetStorageBucket("ml-vision-f0b65.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
                database = FirebaseDatabase.GetInstance(app);
            } else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

            return database;
        }
    }
}