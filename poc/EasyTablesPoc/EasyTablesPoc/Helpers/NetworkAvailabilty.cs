using System;
using Windows.Networking.Connectivity;

namespace EasyTablesPoc.Helpers
{
    public class NetworkAvailabilty
    {
        private static NetworkAvailabilty _networkAvailabilty;
        public static NetworkAvailabilty Instance => _networkAvailabilty ?? (_networkAvailabilty = new NetworkAvailabilty());

        public event Action<bool> OnNetworkAvailabilityChange = delegate { };

        private bool _isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get
            {
                return _isNetworkAvailable;
            }
            protected set
            {
                if (_isNetworkAvailable != value)
                {
                    _isNetworkAvailable = value;
                    OnNetworkAvailabilityChange(value);
                }
            }
        }

        private void CheckInternetAccess()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            IsNetworkAvailable = (connectionProfile != null &&
                                 connectionProfile.GetNetworkConnectivityLevel() ==
                                 NetworkConnectivityLevel.InternetAccess);
        }

        private void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            CheckInternetAccess();
        }

        private NetworkAvailabilty()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;
            CheckInternetAccess();
        }
    }
}
