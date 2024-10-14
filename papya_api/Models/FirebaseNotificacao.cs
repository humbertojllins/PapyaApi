namespace papya_api.Models
{
    public class FirebaseNotificacao
    {
    }
    public class Data
    {
        public string body
        {
            get;
            set;
        }

        public string title
        {
            get;
            set;
        }

        public string image
        {
            get;
            set;
        }

        public string key_1
        {
            get;
            set;
        }

        public string key_2
        {
            get;
            set;
        }

    }

    public class Message
    {

        public string token
        {
            get;
            set;
        }

        public Data data
        {
            get;
            set;
        }

        public Notification notification
        {
            get;
            set;
        }
        public Android android
        {
            get;
            set;
        }
        public Apns apns
        {
            get;
            set;
        }

    }

    public class Notification
    {

        public string title
        {
            get;
            set;
        }

        public string body
        {
            get;
            set;
        }
    }
    public class Root
    {

        public Message message
        {
            get;
            set;
        }

    }
    public class Android
    {
        public NotificacaoAndroid notification
        {
            get;
            set;
        }
    }
    public class Apns
    {
        public Payload payload
        {
            get;
            set;
        }
        public Fcm_options fcm_options
        {
            get;
            set;
        }
        
    }

    public class NotificacaoAndroid
    {
        public string sound
        {
            get;
            set;
        }
        public string image
        {
            get;
            set;
        }
    }
    public class Payload
    {
        public Aps aps
        {
            get;
            set;
        }
    }
    public class Fcm_options
    {
        public string image
        {
            get;
            set;
        }
    }
    public class Aps
    {
        public string sound
        {
            get;
            set;
        }
        public int mutablecontent
        {
            get;
            set;
        }

        public bool default_vibrate_timings
        {
            get;
            set;
        }
        public string[] vibrate_timings
        {
            get;
            set;
        }

    }


}
