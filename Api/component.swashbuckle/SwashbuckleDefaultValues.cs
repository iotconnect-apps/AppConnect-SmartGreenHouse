using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace component.swashbuckle
{
    public class SwashbuckleDefaultValues : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = context.ApiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
                var routInfo = description.RouteInfo;
                if (string.IsNullOrEmpty(parameter.Description))
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }
                if (routInfo == null)
                {
                    continue;
                }
                if (parameter.Default == null)
                {
                    parameter.Default = routInfo.DefaultValue;
                }
                parameter.Required |= !routInfo.IsOptional;
            }
        }
    }
}