
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using SharedShippingDomainsObjects.ValueObjects;


namespace SpotCharterViewModel
{
    public interface ISpotCharterQueryRepository
    {
        IQueryable<SpotCharterView> Find();
        IQueryable<SpotCharterView> Find(Expression<Func<SpotCharterView, bool>> predicate);
        SpotCharterView GetBySpotCharterId(SpotCharterId spotId);
    }
}