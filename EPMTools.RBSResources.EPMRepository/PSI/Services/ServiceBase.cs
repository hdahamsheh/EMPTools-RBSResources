using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace EPMTools.RBSResources.EPMRepository.PSI.Services
{
    public abstract class ServiceBase<T>
    {
        protected T client;
        protected string pwaURL, username, password, domain;

        protected Binding GetBinding(bool isHttps)
        {
            BasicHttpBinding binding = new BasicHttpBinding()
            {
                SendTimeout = new TimeSpan(1, 0, 0),
                MaxBufferPoolSize = 500000000,
                MaxReceivedMessageSize = 500000000,
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                {
                    MaxDepth = 32,
                    MaxStringContentLength = 8192,
                    MaxArrayLength = 16384,
                    MaxBytesPerRead = 4096,
                    MaxNameTableCharCount = 500000000
                }
            };
            if (isHttps)
            {
                binding.Security = new BasicHttpSecurity()
                {
                    Mode = BasicHttpSecurityMode.Transport,
                    Transport = new HttpTransportSecurity()
                    {
                        ClientCredentialType = HttpClientCredentialType.Windows
                    }
                };
                binding.UseDefaultWebProxy = true;
            }
            else {
                binding.Security = new BasicHttpSecurity()
                {
                    Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport = new HttpTransportSecurity()
                    {
                        ClientCredentialType = HttpClientCredentialType.Ntlm,
                        Realm = ""
                    }
                };
            }

            return binding;
        }
        protected EndpointAddress GetAddress()
        {
            string serviceIrl = "/_vti_bin/PSI/ProjectServer.svc";

            EndpointAddress address = new EndpointAddress(this.pwaURL + serviceIrl);
            return address;
        }
    }
}
