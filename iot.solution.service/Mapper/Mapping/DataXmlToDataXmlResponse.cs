using AutoMapper;
using System.Linq;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class DataXmlToDataXmlResponse : ITypeConverter<IOT.DataXml, Entity.DataXml>
    {
        public Entity.DataXml Convert(IOT.DataXml source, Entity.DataXml destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new Entity.DataXml();
            }

            destination.Roleguids = Mapper.Configuration.Mapper.Map<IOT.Roleguids, Entity.Roleguid>(source.roleguids);
            destination.Userguids = Mapper.Configuration.Mapper.Map<IOT.Userguids, Entity.Userguid>(source.userguids);
            destination.Command = Mapper.Configuration.Mapper.Map<IOT.EventCommand, Entity.EventCommand>(source.command);
            if (source.webhook != null)
                destination.Webhook = source.webhook.ToString();
            return destination;
        }
    }
}
