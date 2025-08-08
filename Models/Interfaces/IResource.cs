using Models.Entities;

namespace Models.Interfaces;

public interface IResource
{
    public ResourceState State { get; set; }
    
}