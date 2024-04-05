using System.Collections.Generic;

namespace BeABachelor.Networking.Interface
{
    public interface ISynchronizationController
    {
        void Register(MonoSynchronization monoSynchronization);
        public List<MonoSynchronization> MonoSynchronizations { get; }
    }
}