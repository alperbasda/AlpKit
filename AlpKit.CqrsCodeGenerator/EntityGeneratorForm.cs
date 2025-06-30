using System.Data;
using System.Text;

namespace AlpKit.CqrsCodeGenerator
{
    public partial class EntityGeneratorForm : Form
    {
        public EntityGeneratorForm()
        {
            InitializeComponent();
            cmbType.DataSource = new List<string> { "Guid", "string", "int", "bool", "DateTime", "decimal", "byte[]" };
            cmbCqrsType.DataSource = new List<string> { "CreateCommand", "UpdateCommand", "DeleteByIdCommand", "ListDynamicQuery", "GetByIdQuery" };
            cmbCqrsType.SelectedIndexChanged += cmbCqrsType_SelectedIndexChanged!;
            btnGenerateValidator.Enabled = true;
        }

        private void cmbCqrsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = cmbCqrsType.SelectedItem?.ToString();
            btnGenerateValidator.Enabled = selected is "CreateCommand" or "UpdateCommand";
        }

        private void btnAddProperty_Click(object sender, EventArgs e)
        {
            var name = txtPropName.Text.Trim();
            var type = cmbType.SelectedItem?.ToString() ?? "string";
            if (!string.IsNullOrWhiteSpace(name))
            {
                lstProperties.Items.Add($"{type} {name}");
                txtPropName.Clear();
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var entityName = txtEntityName.Text.Trim();
            if (string.IsNullOrWhiteSpace(entityName)) return;

            var props = GetProperties();

            var sb = new StringBuilder();
            sb.AppendLine(GenerateEntity(entityName, props));
            sb.AppendLine();
            sb.AppendLine(GenerateConfiguration(entityName, props));
            txtResult.Text = sb.ToString();
        }

        private void btnGenerateCqrs_Click(object sender, EventArgs e)
        {
            var entityName = txtEntityName.Text.Trim();
            if (string.IsNullOrWhiteSpace(entityName)) return;

            var props = GetProperties();

            var selected = cmbCqrsType.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected)) return;

            var result = selected switch
            {
                "CreateCommand" => GenerateCreateCommand(entityName, props),
                "UpdateCommand" => GenerateUpdateCommand(entityName, props),
                "DeleteByIdCommand" => GenerateDeleteCommand(entityName),
                "ListDynamicQuery" => GenerateListQuery(entityName, props),
                "GetByIdQuery" => GenerateGetByIdQuery(entityName, props),
                _ => string.Empty
            };

            txtResult.Text = result;
        }

        private void btnGenerateRepo_Click(object sender, EventArgs e)
        {
            var entityName = txtEntityName.Text.Trim();
            var dbContextName = txtDbContextName.Text.Trim();

            if (string.IsNullOrWhiteSpace(entityName) || string.IsNullOrWhiteSpace(dbContextName))
            {
                MessageBox.Show("Lütfen Entity ve DbContext adını girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"public interface I{entityName}Repository : IAsyncRepository<{entityName}>");
            sb.AppendLine("{");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"public class {entityName}Repository({dbContextName} context)");
            sb.AppendLine($"    : RepositoryBase<{entityName}, {dbContextName}>(context), I{entityName}Repository");
            sb.AppendLine("{");
            sb.AppendLine("}");

            txtResult.Text = sb.ToString();
        }

        private void btnGenerateValidator_Click(object sender, EventArgs e)
        {
            var entityName = txtEntityName.Text.Trim();
            if (string.IsNullOrWhiteSpace(entityName)) return;

            var props = new List<(string Type, string Name)>();
            foreach (var item in lstProperties.Items)
            {
                var line = item.ToString();
                if (line!.Trim().StartsWith("public ")) continue;
                var parts = line.Split(' ');
                if (parts.Length == 2)
                    props.Add((parts[0], parts[1]));
            }

            txtResult.Text = GenerateValidator(entityName, props);
        }


        private List<(string Type, string Name)> GetProperties()
        {
            var props = new List<(string Type, string Name)>();
            foreach (var item in lstProperties.Items)
            {
                var parts = item.ToString()?.Split(' ');
                if (parts?.Length == 2)
                    props.Add((parts[0], parts[1]));
            }
            return props;
        }

        private string GenerateEntity(string name, List<(string Type, string Name)> props)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public class {name} : Entity");
            sb.AppendLine("{");
            foreach (var p in props)
                sb.AppendLine($"    public {p.Type} {p.Name} {{ get; set; }} = default!;");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateConfiguration(string name, List<(string Type, string Name)> props)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public class {name}Configuration : IEntityTypeConfiguration<{name}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public void Configure(EntityTypeBuilder<{name}> builder)");
            sb.AppendLine("    {");
            sb.AppendLine($"        builder.ToTable(\"{name}s\");");
            sb.AppendLine("        builder.HasKey(x => x.Id);");
            sb.AppendLine("        builder.Property(x => x.Id).ValueGeneratedNever();");

            sb.AppendLine($"        builder.Property(x => x.CreatedTime);");
            sb.AppendLine($"        builder.Property(x => x.UpdatedTime);");
            sb.AppendLine($"        builder.Property(x => x.DeletedTime);");
            foreach (var p in props)
                sb.AppendLine($"        builder.Property(x => x.{p.Name});");
            
            sb.AppendLine("        builder.HasQueryFilter(w => !w.DeletedTime.HasValue);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateCreateCommand(string entity, List<(string Type, string Name)> props)
        {
            var propsList = string.Join(", ", props.Select(p => $"{p.Type} {p.Name}"));
            var responseFields = string.Join(", ", new[] { "Guid Id" }.Concat(props.Select(p => $"{p.Type} {p.Name}")));

            var sb = new StringBuilder();
            sb.AppendLine($"public record Create{entity}Command({propsList}) : IServiceRequest<Create{entity}Response>;\n");

            sb.AppendLine($"public record Create{entity}Response({responseFields}, DateTime CreatedTime);\n");

            sb.AppendLine($"public class Create{entity}CommandHandler({entity}BusinessRules _businessRules, I{entity}Repository _repository, IMapper _mapper) : IServiceRequestHandler<Create{entity}Command, Create{entity}Response>\n{{\n");
            sb.AppendLine("    public async Task<Response<Create" + entity + "Response>> Handle(Create" + entity + "Command request, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine("        var data = _mapper.Map<" + entity + ">(request);");
            sb.AppendLine("        _businessRules.SetId(data);");
            sb.AppendLine("        await _repository.AddAsync(data);");
            sb.AppendLine("        return Response<Create" + entity + "Response>.Success(_mapper.Map<Create" + entity + "Response>(data), HttpStatusCode.OK);");
            sb.AppendLine("    }");
            sb.AppendLine("}\n");

            sb.AppendLine($"public static class Create{entity}EndpointExtension\n{{\n");
            sb.AppendLine($"    public static RouteGroupBuilder Create{entity}Endpoint(this RouteGroupBuilder group)\n    {{\n");
            sb.AppendLine($"        group.MapPost(\"/\", async ([FromBody] Create{entity}Command command, [FromServices] IMediator mediatr) =>\n        {{\n            return (await mediatr.Send(command)).ToResult();\n        }})");
            sb.AppendLine($"        .AddEndpointFilter(new AuthorizationFilter())");
            sb.AppendLine($"        .AddEndpointFilter<FluentValidationFilter<Create{entity}Command>>();\n");
            sb.AppendLine($"        return group;\n    }}\n}}");

            return sb.ToString();
        }

        private string GenerateUpdateCommand(string entity, List<(string Type, string Name)> props)
        {
            var allProps = new List<(string, string)> { ("Guid", "Id") };
            allProps.AddRange(props);

            var propsList = string.Join(", ", allProps.Select(p => $"{p.Item1} {p.Item2}"));
            var responseFields = string.Join(", ", allProps.Select(p => $"{p.Item1} {p.Item2}"));

            var sb = new StringBuilder();
            sb.AppendLine($"public record Update{entity}Command({propsList}) : IServiceRequest<Update{entity}Response>;\n");
            sb.AppendLine($"public record Update{entity}Response({responseFields});\n");

            sb.AppendLine($"public class Update{entity}CommandHandler({entity}BusinessRules _businessRules, I{entity}Repository _repository, IMapper _mapper) : IServiceRequestHandler<Update{entity}Command, Update{entity}Response>\n{{\n");
            sb.AppendLine("    public async Task<Response<Update" + entity + "Response>> Handle(Update" + entity + "Command request, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine("        var data = await _repository.GetAsync(w => w.Id == request.Id);");
            sb.AppendLine("        await _businessRules.ThrowExceptionIfDataNull(data);");
            sb.AppendLine("        _mapper.Map(request, data);");
            sb.AppendLine("        await _repository.UpdateAsync(data!);");
            sb.AppendLine("        return Response<Update" + entity + "Response>.Success(_mapper.Map<Update" + entity + "Response>(data), HttpStatusCode.OK);");
            sb.AppendLine("    }");
            sb.AppendLine("}\n}\n");

            sb.AppendLine($"public static class Update{entity}EndpointExtension\n{{\n");
            sb.AppendLine($"    public static RouteGroupBuilder Update{entity}Endpoint(this RouteGroupBuilder group)\n    {{\n");
            sb.AppendLine($"        group.MapPut(\"/\", async ([FromBody] Update{entity}Command command, [FromServices] IMediator mediatr) =>\n        {{\n            return (await mediatr.Send(command)).ToResult();\n        }})");
            sb.AppendLine($"        .AddEndpointFilter(new AuthorizationFilter(ApplicationScopes.Admin))");
            sb.AppendLine($"        .AddEndpointFilter<FluentValidationFilter<Update{entity}Command>>();\n");
            sb.AppendLine($"        return group;\n    }}\n}}");

            return sb.ToString();
        }

        private string GenerateDeleteCommand(string entity)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public record DeleteById{entity}Command(Guid Id) : IServiceRequest<DeleteById{entity}Response>;\n");
            sb.AppendLine($"public record DeleteById{entity}Response(Guid Id);\n");

            sb.AppendLine($"public class DeleteById{entity}CommandHandler({entity}BusinessRules _businessRules, I{entity}Repository _repository, IMapper _mapper) : IServiceRequestHandler<DeleteById{entity}Command, DeleteById{entity}Response>\n{{");
            sb.AppendLine("    public async Task<Response<DeleteById" + entity + "Response>> Handle(DeleteById" + entity + "Command request, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine("        var data = await _repository.GetAsync(w => w.Id == request.Id, cancellationToken: cancellationToken);");
            sb.AppendLine("        await _businessRules.ThrowExceptionIfDataNull(data);");
            sb.AppendLine("        await _repository.DeleteAsync(data!);");
            sb.AppendLine("        return Response<DeleteById" + entity + "Response>.Success(_mapper.Map<DeleteById" + entity + "Response>(data), HttpStatusCode.OK);");
            sb.AppendLine("    }");
            sb.AppendLine("}\n");

            sb.AppendLine($"public static class DeleteById{entity}EndpointExtension\n{{");
            sb.AppendLine($"    public static RouteGroupBuilder DeleteById{entity}Endpoint(this RouteGroupBuilder group)\n    {{");
            sb.AppendLine($"        group.MapDelete(\"/{{id:Guid}}\", async ([FromRoute] Guid id, [FromServices] IMediator mediatr) =>\n        {{\n            return (await mediatr.Send(new DeleteById{entity}Command(id))).ToResult();\n        }})");
            sb.AppendLine($"        .AddEndpointFilter(new AuthorizationFilter(ApplicationScopes.Admin));");
            sb.AppendLine($"        return group;\n    }}\n}}");

            return sb.ToString();
        }

        private string GenerateListQuery(string entity, List<(string Type, string Name)> props)
        {
            var sb = new StringBuilder();

            // Command
            sb.AppendLine($"public class ListDynamic{entity}Query : BaseDynamicRequest, IServiceRequest<ListModel<ListDynamic{entity}Response>>");
            sb.AppendLine("{");
            sb.AppendLine("}");
            sb.AppendLine();

            // Response
            sb.AppendLine($"public record ListDynamic{entity}Response({string.Join(", ", props.Select(p => $"{p.Type} {p.Name}"))});");
            sb.AppendLine();

            // Handler
            sb.AppendLine($"public class ListDynamic{entity}QueryHandler({entity}BusinessRules _businessRules, I{entity}Repository _repository, IMapper _mapper) : IServiceRequestHandler<ListDynamic{entity}Query, ListModel<ListDynamic{entity}Response>>");
            sb.AppendLine("{");
            sb.AppendLine($"    public async Task<Response<ListModel<ListDynamic{entity}Response>>> Handle(ListDynamic{entity}Query request, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var data = await _repository.ListDynamicAsync(request.DynamicQuery, size: request.PageRequest.PageSize, index: request.PageRequest.PageIndex, enableTracking: false, cancellationToken: cancellationToken);");
            sb.AppendLine();
            sb.AppendLine($"        var returnData = _mapper.Map<ListModel<ListDynamic{entity}Response>>(data);");
            sb.AppendLine();
            sb.AppendLine($"        _businessRules.FillFilters(returnData, request.DynamicQuery, request.PageRequest);");
            sb.AppendLine();
            sb.AppendLine($"        return Response<ListModel<ListDynamic{entity}Response>>.Success(returnData, HttpStatusCode.OK);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();

            // Endpoint
            sb.AppendLine($"public static class ListDynamic{entity}EndpointExtension");
            sb.AppendLine("{");
            sb.AppendLine($"    public static RouteGroupBuilder ListDynamic{entity}Endpoint(this RouteGroupBuilder group)");
            sb.AppendLine("    {");
            sb.AppendLine($"        group.MapPost(\"/list\", async ([FromServices] IMediator mediatr, HttpContext context) =>");
            sb.AppendLine("        {");
            sb.AppendLine($"            var nvc = context.Request.QueryString.ToNvc();");
            sb.AppendLine($"            return (await mediatr.Send(new ListDynamic{entity}Query\r\n            {{\r\n                DynamicQuery = nvc.ToDynamicFilter<{entity}>(),\r\n                    PageRequest = nvc.ToPageRequest(),\r\n            }})).ToResult();");
            sb.AppendLine("        })");
            sb.AppendLine("        .AddEndpointFilter(new AuthorizationFilter(ApplicationScopes.Admin));");
            sb.AppendLine("        return group;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateGetByIdQuery(string entity, List<(string Type, string Name)> props)
        {
            var sb = new StringBuilder();

            // Query
            sb.AppendLine($"public record GetById{entity}Query(Guid Id) : IServiceRequest<GetById{entity}Response>;");
            sb.AppendLine();

            // Nested response'lar
            var nestedRecords = new List<string>();
            var responseProps = new List<string>();

            foreach (var p in props)
            {
                if (p.Type.StartsWith("List<"))
                {
                    var nestedType = p.Type[5..^1];
                    nestedRecords.Add($"public record {nestedType}(/* TODO: fields */);");
                    responseProps.Add($"{p.Type}? {p.Name} = null");
                }
                else
                {
                    responseProps.Add($"{p.Type} {p.Name}");
                }
            }

            // Response record
            sb.AppendLine($"public record GetById{entity}Response({string.Join(", ", responseProps)});");
            var listProps = props.Where(p => p.Type.StartsWith("List<")).ToList();
            if (listProps.Any())
            {
                sb.AppendLine("{");
                foreach (var listProp in listProps)
                {
                    var innerType = listProp.Type[5..^1];
                    sb.AppendLine($"    public List<{innerType}> {listProp.Name} {{ get; init; }} = {listProp.Name} ?? [];");
                }
                sb.AppendLine("}");
            }
            sb.AppendLine();

            // Handler
            sb.AppendLine($"public class GetById{entity}QueryHandler({entity}BusinessRules _businessRules, I{entity}Repository _repository, IMapper _mapper) : IServiceRequestHandler<GetById{entity}Query, GetById{entity}Response>");
            sb.AppendLine("{");
            sb.AppendLine($"    public async Task<Response<GetById{entity}Response>> Handle(GetById{entity}Query request, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var data = await _repository.GetAsync(w => w.Id == request.Id, enableTracking: false, cancellationToken: cancellationToken);");
            sb.AppendLine($"        await _businessRules.ThrowExceptionIfDataNull(data);");
            sb.AppendLine($"        return Response<GetById{entity}Response>.Success(_mapper.Map<GetById{entity}Response>(data), HttpStatusCode.OK);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();

            // Endpoint
            sb.AppendLine($"public static class GetById{entity}EndpointExtension");
            sb.AppendLine("{");
            sb.AppendLine($"    public static RouteGroupBuilder GetById{entity}Endpoint(this RouteGroupBuilder group)");
            sb.AppendLine("    {");
            sb.AppendLine($"        group.MapGet(\"/{{id:Guid}}\", async ([FromRoute] Guid id, [FromServices] IMediator mediatr) =>");
            sb.AppendLine("        {");
            sb.AppendLine($"            return (await mediatr.Send(new GetById{entity}Query(id))).ToResult();");
            sb.AppendLine("        })");
            sb.AppendLine("        .AddEndpointFilter(new AuthorizationFilter(ApplicationScopes.Admin));");
            sb.AppendLine("        return group;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Nested record'lar (List<T> olanlar)
            foreach (var rec in nestedRecords)
            {
                sb.AppendLine();
                sb.AppendLine(rec);
            }

            return sb.ToString();
        }

        private string GenerateValidator(string entity, List<(string Type, string Name)> props)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"public class Create{entity}CommandValidator : AbstractValidator<Create{entity}Command>");
            sb.AppendLine("{");
            sb.AppendLine($"    public Create{entity}CommandValidator(IDistributedCache distributedCache, TokenParameters tokenParameters)");
            sb.AppendLine("    {");

            foreach (var p in props)
            {
                sb.AppendLine($"        RuleFor(w => w.{p.Name})");
                sb.AppendLine("            .CustomAsync(async (obj, context, cancellationToken) => await FluentValidationCustomRules.IsNullOrEmpty(distributedCache, obj, context, tokenParameters.UserLanguage, cancellationToken));");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
    
}
