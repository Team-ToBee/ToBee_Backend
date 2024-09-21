using AutoMapper;
using ToBee.API.Dtos.RewardDtos;
using ToBee.API.Dtos.TaskServiceDtos;
using ToBee.API.Models;

namespace ToBee.API.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<TaskService, TaskServiceDto>().ReverseMap();

			CreateMap<Reward, RewardDto >().ReverseMap();
		}
	}
}
