using AutoMapper;
using Entity = iot.solution.entity;
using IOT = IoTConnect.Model;

namespace iot.solution.service.Mapper.Mapping
{
    public class SingleRuleResultToSingleRuleResponse : ITypeConverter<IOT.SingleRuleResult, Entity.SingleRuleResponse>
    {
        public Entity.SingleRuleResponse Convert(IOT.SingleRuleResult source, Entity.SingleRuleResponse destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }

            if (destination == null)
            {
                destination = new Entity.SingleRuleResponse();
            }


            destination.Roles = source.roles;
            destination.Tag = source.tag;
            destination.EventSubscription = Mapper.Configuration.Mapper.Map<IOT.EventSubscription, Entity.EventSubscription>(source.eventSubscription); 
            destination.EventSubscriptionGuid = source.eventSubscriptionGuid;
            destination.IsActive = source.isActive;
            destination.ApplyTo = source.applyTo;
            destination.EntityGuid = source.entityGuid;
            destination.IgnorePreference = source.ignorePreference;
            destination.ConditionText = source.conditionText;
            destination.Name = source.name;
            destination.ParentAttributeGuid = source.parentAttributeGuid;
            destination.RuleType = source.ruleType;
            destination.AttributeGuid = source.attributeGuid;
            destination.TemplateGuid = source.templateGuid;
            destination.CompanyGuid = source.companyGuid;
            destination.Guid = source.guid;
            destination.Users = source.users;
            destination.Devices = source.devices;

            return destination;
        }
    }
}
