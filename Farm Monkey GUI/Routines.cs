using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcheBuddy.Bot.Classes;

namespace FarmMonkey_GUI
{
    public partial class MainWindow
    {
        #region Global Variables
        /// <summary>
        /// Host is required to run
        /// </summary>
        private Core Host;
        public uint[] _badSkillId = {
                              13166, // Light
                              13789, // Uproot
                              13967, // Butcher (Hen, 10 labor)
                            //13973, // Butcher (Halcyona Hen, 5 Labor)
                              16768, // Needless Butchering
                              16828, // Open or Close Building Door
                              18025, // Light Fireplace
                              19383, // Get Water
                              20782  // Butcher (Small Chicken Coop, 50 labor)
                             };
        public uint[] _farmGroups = {
                               8, // Saplings
                              21, // Livestock
                              47, // Grain
                              48, // Vegetable
                              49, // Fruit
                              51, // Seed
                              52, // Spice
                              53, // Herb
                              55  // Flower
                             };
        // Gather Farm ID's wtih scarecrow { 12345, 54321 }
        public List<Housing> _farms = new List<Housing>();
        /// <summary>
        /// Required for traveling functions
        /// </summary>
        public Gps gps;
        #endregion

        public void SetHost(Core core)
        {
            Host = core;
        }
        // Farming Routines
        public async Task HarvestingAsync()
        {
            await Task.Run(new Action(Harvesting));
            //await (new Task(new Action(Harvesting)));
            //return;
        }
        public void Harvesting()
        {
            try
            {
                var _labor = Host.me.laborPoints;
                Host.Log(_Time() + "Current Labor:" + _labor);
                if (_labor > Config.MinLabor)
                {
                    foreach (Housing farm in _farms)
                    {

                        List<HarvestingCollectionItem> Harvestable = new List<HarvestingCollectionItem>();
                        foreach (DoodadObject doodad in getMyDoodads())
                        {
                            foreach (Skill skill in doodad.getUseSkills())
                            {
                                if (!_badSkillId.Contains(skill.id) && (from harvestitem in Harvestable
                                                                        where harvestitem.SkillId == skill.id
                                                                        select harvestitem).Count() == 0)
                                {
                                    Harvestable.Add(new HarvestingCollectionItem
                                    {
                                        DoodadName = doodad.name,
                                        PhaseId = doodad.phaseId,
                                        SkillId = skill.id
                                    });
                                    switch (doodad.phaseId)
                                    {
                                        /* Name: Thriving Hen
                                         * PhaseId: 5798
                                         * SkillId: 13801 */
                                        case 5794: // Hen
                                            if ((from harvestitem in Harvestable
                                                 where harvestitem.SkillId == 13801
                                                 select harvestitem).Count() == 0)
                                                Harvestable.Add(new HarvestingCollectionItem
                                                {
                                                    DoodadName = "Thriving Hen",
                                                    PhaseId = 5798,
                                                    SkillId = 13801
                                                });
                                            break;
                                        /* Name: Thriving Chicken Coop
                                         * PhaseId: 16347
                                         * SkillId: 20784 */
                                        case 16344:
                                            if ((from harvestitem in Harvestable
                                                 where harvestitem.SkillId == 20784
                                                 select harvestitem).Count() == 0)
                                                Harvestable.Add(new HarvestingCollectionItem
                                                {
                                                    DoodadName = "Thriving Chicken Coop",
                                                    PhaseId = 16347,
                                                    SkillId = 20784
                                                });
                                            break;
                                    }
                                }
                            }
                        }
                        // Loop through all the harvestable doodads that we found.
                        foreach (HarvestingCollectionItem harvestMe in Harvestable)
                        {
                            if (harvestMe.SkillId == 13625) continue; // Don't try to water plants.
                            Host.Log(_Time() + "[INFO] " + "Harvesting | Skill ID: " + harvestMe.SkillId + " Doodad Name: " + harvestMe.DoodadName);
                            Host.CollectItemsAtFarm(harvestMe.PhaseId, harvestMe.SkillId, farm.uniqHousingId);
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Host.Log(e.ToString());
            }
        }



        public async Task PlantingAsync()
        {
            await Task.Run(new Action(Planting));
        }
        public void Planting()
        {
            try
            {
                foreach (Housing farm in _farms)
                {
                    foreach (var item in Host.sqlCore.sqlItems)
                    {
                        if (_farmGroups.Contains(item.Value.itemCategorie.id) && Host.itemCount(item.Value.id) > 0 && item.Value.implId == 9)
                        {
                            if (Host.itemCount(item.Value.id) == 0)
                            {
                                Host.Log(_Time() + "[Warning] " + "You have no seeds!");
                                return;
                            }
                            Host.Log(_Time() + "Planting " + item.Value.name + "(s) [" + Host.itemCount(item.Value.name) + "] on FarmID: " + farm.uniqHousingId);
                            for (uint i = 0; i <= Host.itemCount(item.Value.name); i++)
                            {
                                if (Host.PlantItemsAtFarm(item.Value.id, farm.uniqHousingId))
                                { // Provide time for GUI updates
                                    //Task.Delay(random.Next(10, 125));
                                }
                                else break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Host.Log(e.ToString());
            }
        }

        #region Utility Stuff

        public void GetFarms()
        {
            _farms.Clear();
            List<Creature> creatureList = Host.getCreatures().Where(x => x.type == BotTypes.Housing).ToList();
            Parallel.ForEach(creatureList, new Action<Creature>(farm =>
            {
                Housing _farm = farm as Housing;
                Parallel.ForEach(Host.me.family.getMembers(), new Action<FamilyMember>(x =>
                {
                    if ((_farm.ownerName == x.name) && (_farm.access == HouseAccess.Guild || _farm.access == HouseAccess.Family))
                    {
                        if (!_farms.Contains(_farm)) _farms.Add(_farm);
                    }
                }));
                if (_farm.ownerName == Host.me.name) if (!_farms.Contains(_farm)) _farms.Add(_farm);
            }));
        }
        public string _Time()
        {
            return DateTime.Now.ToString("[hh:mm:ss] ");
        }

        public IEnumerable<DoodadObject> getMyDoodads()
        //public List<DoodadObject> getMyDoodads()
        {
            //List<DoodadObject> myDoodads = new List<DoodadObject>();
            if (_farms.Count == 0) GetFarms();
            return (from doodad in Host.getDoodads()
                    where (from farm in _farms
                           select farm.plantZoneId)
                           .Contains(doodad.plantZoneId)
                    select doodad);
        }

        #endregion Utility Stuff
    }

    class HarvestingCollectionItem
    {
        public uint SkillId { get; set; }
        public uint PhaseId { get; set; }
        public string DoodadName { get; set; }
    }
}
