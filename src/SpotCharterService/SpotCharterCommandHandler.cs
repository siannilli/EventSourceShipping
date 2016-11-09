using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaseDomainObjects;
using SpotCharterDomain;
using SpotCharterServices.Commands;

namespace SpotCharterServices
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

        private readonly ISpotCharterRepository  _repository;

        public SpotCharterCommandHandler(ISpotCharterRepository repository)
        {
            this._repository = repository;
        }

        void ICommandHandler<ChangeDemurrageRate>.Handle(ChangeDemurrageRate command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeDemurrageRate(command.LaytimeLoad, command.LaytimeDischarge, command.LaytimeTotal, command.Rate, command.TimeUnit);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeVessel>.Handle(ChangeVessel command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeVessel(command.VesselId, command.Name);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangePortfolio>.Handle(ChangePortfolio command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangePortfolio(command.PortfolioId);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeLaycan>.Handle(ChangeLaycan command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeLaycan(command.From, command.To);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeBillOfLading>.Handle(ChangeBillOfLading command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeBillOfLading(command.BLDate, command.BLQuantity, command.DocumentReference);
            this._repository.Save(spot);
        }

        void ICommandHandler<ChangeCharterparty>.Handle(ChangeCharterparty command)
        {
            var spot = this._repository.Get(command.SpotCharterId);
            spot.ChangeCharterparty(command.CharterpartyId, command.Name);
            this._repository.Save(spot);

        }

        void ICommandHandler<CreateSpotCharter>.Handle(CreateSpotCharter command)
        {
            var spot = new SpotCharter(command.CharterpartyDate, command.CharterpartyId, command.CharterpartyName, command.VesselId, command.VesselName, command.MinimumQuantity);
            this._repository.Save(spot);
        }

    }
}
