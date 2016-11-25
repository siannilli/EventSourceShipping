using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects;
using SpotCharterDomain;
using SpotCharterDomain.Commands;

namespace SpotCharterDomain
{
    public class SpotCharterCommandHandler :
        ICommandHandler<CreateSpotCharter>,
        ICommandHandler<ChangeCharterparty>,
        ICommandHandler<ChangeBillOfLading>,
        ICommandHandler<ChangeLaycan>,
        ICommandHandler<ChangeDemurrageRate>,
        ICommandHandler<ChangePortfolio>,
        ICommandHandler<ChangeVessel>

    {

        private readonly ISpotCharterCommandRepository  _repository;

        public SpotCharterCommandHandler(ISpotCharterCommandRepository repository)
        {
            this._repository = repository;
        }

        void ICommandHandler<ChangeDemurrageRate>.Handle(ChangeDemurrageRate command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeDemurrageRate(command);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeVessel>.Handle(ChangeVessel command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeVessel(command);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangePortfolio>.Handle(ChangePortfolio command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangePortfolio(command);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeLaycan>.Handle(ChangeLaycan command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeLaycan(command);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeBillOfLading>.Handle(ChangeBillOfLading command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeBillOfLading(command);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeCharterparty>.Handle(ChangeCharterparty command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeCharterparty(command);
            this._repository.Save(spot);

        }

        void ICommandHandler<CreateSpotCharter>.Handle(CreateSpotCharter command)
        {
            var spot = new SpotCharter(command);
            this._repository.Save(spot);
        }

    }
}
