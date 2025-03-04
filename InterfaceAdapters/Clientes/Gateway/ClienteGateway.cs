using CasosDeUso.Clientes.Interfaces.Gateway;
using Entidades.Clientes;
using InterfaceAdapters.Bases.Gateway;
using NHibernate;

namespace InterfaceAdapters.Clientes.Gateway
{
    public class ClienteGateway(ISession session) : BaseGateway<Cliente>(session), IClienteGateway
    {
    }
}
