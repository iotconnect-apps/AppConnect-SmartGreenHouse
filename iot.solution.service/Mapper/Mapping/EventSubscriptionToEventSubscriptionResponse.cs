using AutoMapper;
using System.Linq;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class EventSubscriptionToEventSubscriptionResponse : ITypeConverter<IOT.EventSubscription, Entity.EventSubscription>
    {
        public Entity.EventSubscription Convert(IOT.EventSubscription source, Entity.EventSubscription destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new Entity.EventSubscription();
            }

            destination.Guid = source.guid;
            destination.EventGuid = source.eventGuid;
            destination.DeliveryMethod = source.deliveryMethod;
            destination.DeliveryMethodData = source.deliveryMethodData.Select(g => Mapper.Configuration.Mapper.Map<IOT.DeliveryMethodData, Entity.DeliveryMethodData>(g)).ToList();
            destination.CompanyGuid = source.companyGuid;
            destination.SolutionGuid = source.solutionGuid;
            destination.RefGuid = source.refGuid;
            destination.DataXml = Mapper.Configuration.Mapper.Map<IOT.DataXml, Entity.DataXml>(source.dataXml);
            destination.SeverityLevelGuid = source.severityLevelGuid;
            return destination;
        }
    }
}
