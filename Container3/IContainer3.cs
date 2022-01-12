using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Container3
{
    [ServiceContract]
    public interface IContainer3
    {
        [OperationContract]
        Task<bool> ContainerCall();
    }
}
