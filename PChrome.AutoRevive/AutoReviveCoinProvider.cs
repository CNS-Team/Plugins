﻿using LazyUtils;
using LinqToDB;
using PChrome.Shop;
using TShockAPI;

namespace PChrome.AutoRevive;

internal class AutoReviveCoinProvider : SingleItemProvider
{
    public override string Name => "复活币";
    protected override bool TryTakeFrom(TSPlayer player, int count)
    {
        using var query = player.Get<AutoReviveCoin>();
        if (query.Single().count < count)
        {
            return false;
        }

        query.Set(d => d.count, d => d.count - count).Update();
        return true;
    }

    protected override bool TryGiveTo(TSPlayer player, int stack)
    {
        using var query = player.Get<AutoReviveCoin>();
        query.Set(d => d.count, d => d.count + stack).Update();
        return true;
    }
}