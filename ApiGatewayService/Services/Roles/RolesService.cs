using System.Text;
using BackGazprom.Models.Generic.Responses;
using Confluent.Kafka;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;
using UserService.Exceptions.RoleExceptions;
using UserService.Models.Roles.Requests;
using UserService.Models.Roles.Responses;

namespace UserService.Services.Roles;

public class RolesService : IRolesService
{
    private readonly ILogger<RolesService> _logger;
    private readonly KafkaService _kafkaService;
    public RolesService( ILogger<RolesService> logger, KafkaService kafkaService)
    {
        _logger = logger;
        _kafkaService = kafkaService;
    }
    public async Task<RoleResponse> CreateRole(RoleCreateRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("createRole")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var role = await _kafkaService.Consume<RoleResponse>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "", messageId, "createRole");
                _logger.LogInformation("Role created, Role: {Role}", JsonConvert.SerializeObject(request));
                return role;
            }
            _logger.LogError("Error sending role, Role: {Role}", JsonConvert.SerializeObject(request));
            throw new CreateRoleException("Error sending role, Role: "+JsonConvert.SerializeObject(request));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error creating role, Role: {Role}", JsonConvert.SerializeObject(request));
                throw new CreateRoleException("Error creating role, Role: "+JsonConvert.SerializeObject(request),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<bool> DeleteRole(RoleDeleteRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("deleteRole")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var deleteRoleResponse = await _kafkaService.Consume<MessageResponse>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "", messageId, "deleteRole");
                _logger.LogInformation("Role deleted, Role: {Role}", JsonConvert.SerializeObject(request));
                return !deleteRoleResponse.Message.Contains("Error");
            }
            _logger.LogError("Error sending role, Role: {Role}", JsonConvert.SerializeObject(request));
            throw new DeleteRoleException("Error sending role, Role: "+JsonConvert.SerializeObject(request));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error deleting role, Role: {Role}", JsonConvert.SerializeObject(request));
                throw new DeleteRoleException("Error deleting role, Role: "+JsonConvert.SerializeObject(request),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<List<RoleResponse>> GetRoles()
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("getRoles")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                List<RoleResponse> roles = await _kafkaService.Consume<List<RoleResponse>>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "", messageId, "getRoles");
                _logger.LogInformation("Roles found");
                return roles;
            }
            _logger.LogError("Error getting roles");
            throw new GetRolesException("Error sending roles");
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error getting roles");
                throw new GetRolesException("Error getting roles",e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<RoleResponse> UpdateRole(RoleUpdateRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("updateRole")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var role = await _kafkaService.Consume<RoleResponse>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "", messageId, "updateRole");
                _logger.LogInformation("Role updated, Role: {Role}", JsonConvert.SerializeObject(request));
                return role;
            }
            _logger.LogError("Error sending role, Role: {Role}", JsonConvert.SerializeObject(request));
            throw new UpdateRoleException("Error sending role, Role: "+JsonConvert.SerializeObject(request));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error updating role, Role: {Role}", JsonConvert.SerializeObject(request));
                throw new UpdateRoleException("Error updating role, Role: "+JsonConvert.SerializeObject(request),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }
}
