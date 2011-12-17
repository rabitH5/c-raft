#region C#raft License
// This file is part of C#raft. Copyright C#raft Team 
// 
// C#raft is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chraft.Net;
using Chraft.Plugins;
using Chraft.Utils;
using Chraft.World;
using Chraft.Entity;

namespace Chraft.Commands.Debug
{
    public class DbgMob : IClientCommand
    {
        public ClientCommandHandler ClientCommandHandler { get; set; }

        public void Use(Client client, string commandName, string[] tokens)
        {
            Vector3 facing = new Vector3(client.Owner.Yaw, client.Owner.Pitch);

            Vector3 start = new Vector3(client.Owner.Position.X, client.Owner.Position.Y + client.Owner.EyeHeight, client.Owner.Position.Z);
            Vector3 end = facing * 100 + start;
            if (end.Y < 0)
            {
                end = end * (Math.Abs(end.Y) / start.Y);
                end.Y = 0;
            }

            RayTraceHitBlock hit = client.Owner.World.RayTraceBlocks(new AbsWorldCoords(start), new AbsWorldCoords(end));

            if (hit != null)
            {
                if (tokens.Length == 0)
                {
                }
                else
                {
                    MobType mobType;
                    if (Enum.TryParse<MobType>(tokens[0], true, out mobType))
                    {
                        Mob theMob = MobFactory.CreateMob(client.Owner.World, client.Server.AllocateEntity(), mobType, null);
                        theMob.Position = new AbsWorldCoords(client.Owner.World.FromFace(hit.TargetBlock, hit.FaceHit));
                        client.Server.AddEntity(theMob);
                    }
                    else if (tokens[0] == "update")
                    {
                        foreach (var entity in client.Server.GetNearbyEntities(client.Owner.World, client.Owner.Position))
                        {
                            entity.TeleportTo(entity.Position);
                        }
                    }
                    else
                    {
                        client.SendMessage(String.Format("Unrecognised mob type: '{0}'", tokens[0]));
                    }
                }
            }
        }

        public void Help(Client client)
        {

        }

        public string Name
        {
            get { return "dbgmob"; }
        }

        public string Shortcut
        {
            get { return "dbgmob"; }
        }

        public CommandType Type
        {
            get { return CommandType.Information; }
        }

        public string Permission
        {
            get { return "chraft.debug"; }
        }

        public IPlugin Iplugin { get; set; }
    }
}


