
namespace ApplicationLayer.DTOs
{
    public class UserRolesDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<RoleDTO> Roles { get; set; }
    }
}
