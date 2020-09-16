namespace Heyworks.PocketShooter.Modules.Analytics
{
    public class AnalyticsKeys
    {
        public class EventNames
        {
            public const string BattlePerformance = "battle_performance";
            public const string PaymentSucceed = "payment_succeed";
            public const string HardCurrency = "hard_currency";
            public const string SoftCurrency = "soft_currency";
            public const string EndBattle = "end_battle";
            public const string EndBattleStats = "end_battle_stats";
            public const string PlayerBattleFinish = "player_battle_finish";
            public const string UserLevelUp = "user_lvlup";
            public const string AppLaunch = "app_launch";
            public const string AppClosed = "app_closed";
            public const string Offer = "offer";
            public const string TrooperOpen = "trooper_open";
            public const string Matchmaking = "matchmaking";
            public const string Login = "login";
        }

        public class ParametrsKeys
        {
            //---------- Profile ----------

            public const string ProfileID = "profileID";
            public const string RegDate = "reg_date";
            public const string TestAccount = "test_account";
            public const string AccountLevel = "account_level";
            public const string Battles = "battles";
            public const string Payer = "payer";
            public const string PaidCount = "paid_count";
            public const string PaidTotal = "paid_total";
            public const string HardCurrencyBalance = "hard_currency_balance";
            public const string SoftCurrencyBalance = "soft_currency_balance";

            //--------------------

            public const string AverageFPS = "avg_fps";
            public const string Group_0_10_FPS = "group_0_10_fps";
            public const string Group_10_20_FPS = "group_10_20_fps";
            public const string Group_20_30_FPS = "group_20_30_fps";
            public const string Group_30_40_FPS = "group_30_40_fps";
            public const string Group_40_50_FPS = "group_40_50_fps";
            public const string Group_50_60_FPS = "group_50_60_fps";

            public const string AveragePing = "avg_ping";
            public const string Group_0_100_Ping = "group_0_100_ping";
            public const string Group_100_200_Ping = "group_100_200_ping";
            public const string Group_200_300_Ping = "group_200_300_ping";
            public const string Group_300_500_Ping = "group_300_500_ping";
            public const string Group_500_1000_Ping = "group_500_1000_ping";
            public const string Group_1000_2000_Ping = "group_1000_2000_ping";
            public const string Group_2000_INFINITY_Ping = "group_2000_inf_ping";

            public const string TrooperClass = "TrooperClass";
            public const string Screen = "screen";

            public const string TransactionId = "transaction_id";
            public const string LocalCurrency = "currency";
            public const string LocalPrice = "price";
            public const string DollarPrice = "dollar_price";
            public const string InAppID = "inapp_id";
            public const string ShopCategory = "category";
            public const string ID = "item_id";
            public const string Tr_Type = "tr_type";
            public const string Value = "value";

            public const string Result = "result";
            public const string ResultCondition = "result_cond";
            public const string Kills = "kills";
            public const string Death = "deaths";
            public const string MVP = "mvp";
            public const string SoftReward = "soft_reward";
            public const string HardReward = "hard_reward";
            public const string TroopersList = "troopers_list";
            public const string TroopersLevels = "troopers_levels";
            public const string AIUnits = "ai_units";
            public const string DropReason = "drop_reason";
            public const string Timer = "timer";
            public const string Finish_reason = "finish_reason";

            public const string Name = "name";
            public const string Button = "button";

            public const string Success = "success";
            public const string Location = "location";
            public const string SessionNumber = "session_number";
            public const string SessionBattles = "session_battles";

            public const string TrooperName = "trooper_name";
            public const string TrooperRank = "trooper_rank";
            public const string TrooperLevel = "trooper_level";
            public const string Currency_name = "currency_name";
            public const string PurchaseSource = "purchase_source";

            public const string WaitTime = "wait_time";
            public const string GameMode = "game_mode";
            public const string MatchmakingGroup = "matchmaking_group";

            public const string Map = "map"; 
            public const string Duration = "duration";
        }
    }
}