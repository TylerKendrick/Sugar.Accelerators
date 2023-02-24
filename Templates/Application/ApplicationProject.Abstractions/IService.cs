namespace ApplicationProject.Abstractions;

public interface IService
{
    Task<Person> GetPerson(string name);
}
