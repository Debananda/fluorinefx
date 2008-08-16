/*
	FluorineFx open source library 
	Copyright (C) 2007 Zoltan Csibi, zoltan@TheSilentGroup.com, FluorineFx.com 
	
	This library is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public
	License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.
	
	This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	Lesser General Public License for more details.
	
	You should have received a copy of the GNU Lesser General Public
	License along with this library; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using System;

using FluorineFx.Messaging.Api;
using FluorineFx.Messaging.Api.Service;
using FluorineFx.Context;

namespace FluorineFx.Messaging.Rtmp
{
	/// <summary>
    /// Base IScopeHandler implementation.
	/// </summary>
	class CoreHandler : IScopeHandler
	{
		public CoreHandler()
		{
		}

		#region IScopeHandler Members

		public bool Start(IScope scope)
		{
			return true;
		}

		public void Stop(IScope scope)
		{
			//NA
		}

		public bool Connect(IConnection connection, IScope scope, object[] parameters)
		{
            IScope connectionScope = connection.Scope;
            IClient client = null;
            if (connection.IsFlexClient)
            {
                if (parameters != null && parameters.Length == 3)
                {
                    string clientId = parameters[1] as string;
                    client = connectionScope.Context.ClientRegistry.GetClient(clientId);
                    connection.Initialize(client);
                    client.Renew(connection.ClientLeaseTime);
                    return true;
                }
                if (parameters != null && parameters.Length == 2)
                {
                    string clientId = parameters[1] as string;
                    client = connectionScope.Context.ClientRegistry.GetClient(clientId);
                    connection.Initialize(client);
                    client.Renew(connection.ClientLeaseTime);
                    return true;
                }
            }
            string id = connection.ConnectionId;
            // Use client registry from scope the client connected to.
            IClientRegistry clientRegistry = connectionScope.Context.ClientRegistry;
            client = clientRegistry.GetClient(id);
            //IClient client = clientRegistry.HasClient(id) ? clientRegistry.LookupClient(id) : clientRegistry.NewClient(id, connection.ClientLeaseTime, parameters);
            // We have a context, and a client object.. time to init the conneciton.
            connection.Initialize(client);
            client.Renew(connection.ClientLeaseTime);
            // we could check for banned clients here 
			return true;
		}

		public void Disconnect(IConnection connection, IScope scope)
		{
			//NA
		}

		public bool AddChildScope(IBasicScope scope)
		{
			return true;
		}

		public void RemoveChildScope(IBasicScope scope)
		{
			//NA
		}

		public bool Join(IClient client, IScope scope)
		{
			return true;
		}

		public void Leave(IClient client, IScope scope)
		{
			//NA
		}

		public bool ServiceCall(IConnection connection, IServiceCall call)
		{
			IScopeContext context = connection.Scope.Context;
			if(call.ServiceName != null) 
			{
				context.ServiceInvoker.Invoke(call, context);
			} 
			else 
			{
				context.ServiceInvoker.Invoke(call, connection.Scope.Handler);
			}
			return true;
		}

		#endregion

		#region IEventHandler Members

		public bool HandleEvent(FluorineFx.Messaging.Api.Event.IEvent evt)
		{
			return false;
		}

		#endregion
	}
}
