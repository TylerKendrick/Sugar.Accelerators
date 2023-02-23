namespace Project.Abstractions;

public interface IService
{
    Task<Person> GetPerson(string name);
}
