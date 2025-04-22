using MediatR;
using UserService.UnitOfWork;

namespace UserService.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<Models.User?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Models.User?>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users.GetAllAsync();
        }
    }
}

