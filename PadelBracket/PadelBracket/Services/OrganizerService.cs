using PadelBracket.Domain.DTOs;
using PadelBracket.Domain.Entities;
using PadelBracket.Repositories.Interface;

namespace PadelBracket.Services;

public class OrganizerService
{
    private readonly IOrganizerRepository organizerRepository;

    public OrganizerService(IOrganizerRepository organizerRepository)
    {
        this.organizerRepository = organizerRepository;
    }

    public IReadOnlyList<Organizer> GetAll()
    {
        return organizerRepository.GetAll();
    }

    public IReadOnlyList<OrganizerDto> GetAllDtos()
    {
        return organizerRepository.GetAll()
            .Select(ToDto)
            .ToList();
    }

    public Organizer? GetById(Guid id)
    {
        return organizerRepository.GetById(id);
    }

    public OrganizerDto? GetDtoById(Guid id)
    {
        Organizer? organizer = GetById(id);

        if (organizer == null)
            return null;

        return ToDto(organizer);
    }

    public Organizer Add(
        string realName,
        string email,
        string clubName,
        string city,
        string phone)
    {
        if (organizerRepository.ExistsByEmail(email))
            throw new ArgumentException("An organizer with that email already exists.");

        Organizer organizer = new Organizer(
            realName,
            email,
            clubName,
            city,
            phone);

        organizerRepository.Add(organizer);

        return organizer;
    }

    public void UpdateProfile(
        Guid id,
        string realName,
        string email,
        string clubName,
        string city,
        string phone)
    {
        Organizer organizer = GetById(id)
            ?? throw new ArgumentException("Organizer not found.");

        if (organizerRepository.ExistsByEmailExceptId(email, id))
            throw new ArgumentException("An organizer with that email already exists.");

        organizer.UpdateProfile(
            realName,
            email,
            clubName,
            city,
            phone);
        organizerRepository.SaveChanges();
    }

    public void Delete(Guid id)
    {
        Organizer organizer = GetById(id)
            ?? throw new ArgumentException("Organizer not found.");

        organizerRepository.Delete(organizer);
    }

    private static OrganizerDto ToDto(Organizer organizer)
    {
        return new OrganizerDto
        {
            Id = organizer.Id,
            RealName = organizer.RealName,
            Email = organizer.Email,
            ClubName = organizer.ClubName,
            City = organizer.City,
            Phone = organizer.Phone,
            CreatedAt = organizer.CreatedAt,
            HasCompleteProfile = organizer.HasCompleteProfile
        };
    }
}