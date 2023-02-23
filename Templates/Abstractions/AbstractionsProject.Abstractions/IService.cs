namespace AbstractionsProject.Abstractions;

public interface IService
{
    Task<Person> GetPerson(string name);
}
