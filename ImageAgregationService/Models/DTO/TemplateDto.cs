namespace ImageAgregationService.Models.DTO
{
    public class TemplateDto
    {
        public string Name {get;set;} = null!;
        public string DefaultPrompt {get;set;} = null!;
        public long RoleId {get;set;}
        public string RoleName {get;set;} = null!;
    }
}