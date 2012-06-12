using System;
using System.Collections.Generic;
using System.Linq;
using D3;

namespace d3helper.Bots
{
    class Spot
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        //public float Z { get; private set; }

        public Spot(float _x, float _y/*, float _z*/)
        {
            X = _x;
            Y = _y;
            //Z = _z;
        }
    }

    class RoyalCryptsBot : BotBase
    {
        private long ticks;

        private const float RangedDistance = 40.0f;
        private const float MeleeDistance = 5.0f;

        // actors
        private SNOActorId[] Ashes = {
                                        SNOActorId.trDun_Crypt_Urn_Group_A_01,
                                        SNOActorId.trDun_Crypt_Urn_Group_A_02,
                                        SNOActorId.trDun_Crypt_Urn_Group_A_03
                                     };

        private SNOActorId[] DeadVillagers = {
                                                SNOActorId.LootType2_Adventurer_A_Corpse_01,
                                                SNOActorId.LootType2_Adventurer_A_Corpse_02,
                                                SNOActorId.LootType2_Adventurer_B_Corpse_01,
                                                SNOActorId.LootType2_Adventurer_B_Corpse_02,
                                                SNOActorId.LootType2_Adventurer_C_Corpse_01,
                                                SNOActorId.LootType2_Adventurer_C_Corpse_02,
                                                SNOActorId.LootType2_Adventurer_D_Corpse_01,
                                                SNOActorId.LootType2_Adventurer_D_Corpse_02
                                             };

        private SNOActorId[] Gates = {
                                        SNOActorId.trDun_Cath_Gate_A,
                                        SNOActorId.trDun_Cath_Gate_C,
                                        SNOActorId.trDun_Cath_Gate_D
                                     };

        private SNOActorId[] Doors = {
                                        SNOActorId.trDun_Crypt_Door,
                                        SNOActorId.trDun_Cath_WoodDoor_A_Barricaded,
                                        //SNOActorId.trDun_SkeletonKing_Sealed_Door // door to Skeleton King
                                     };

        private SNOActorId SkeletalArcher = SNOActorId.SkeletonArcher_A;
        private SNOActorId Skeleton = SNOActorId.Skeleton_A;
        private SNOActorId LooseStone = SNOActorId.trDun_Cath_FloorSpawner_02;
        private SNOActorId StoneCoffin = SNOActorId.Crypt_Coffin_Stone_02;
        private SNOActorId Chest = SNOActorId.a1dun_Cath_chest;
        private SNOActorId ScribesLectern = SNOActorId.LeoricManor_Lecturn__Leorics_Journal;
        private SNOActorId Sarcophagus = SNOActorId.trDun_Floor_Sarcophagus;
        private SNOActorId ActivatedPillar = SNOActorId.trDun_Crypt_Pillar_Spawner;

        private List<uint> OpenedVillagers = new List<uint>();

        private List<Spot> Spots = new List<Spot>()
        {
            new Spot(1060, 500), new Spot(1060, 530), new Spot(1060, 565), new Spot(1045, 580),
            new Spot(1010, 580), new Spot(980, 580), new Spot(950, 580), new Spot(920, 580),
            new Spot(890, 580), new Spot(866, 563), new Spot(866, 563), new Spot(860, 540),
            new Spot(820, 525), new Spot(820, 470), new Spot(820, 525), new Spot(780, 540),
            new Spot(770, 580), new Spot(780, 620), new Spot(820, 640), new Spot(860, 620),
            new Spot(820, 640), new Spot(820, 680), new Spot(820, 740), new Spot(820, 680),
            new Spot(820, 640), new Spot(780, 620), new Spot(760, 580)
        };

        private int SpotIndex = 0;

        private bool Started = false;

        // Stone Coffin
        // Sarcophagus
        // Loose Stone
        // Dead Villager
        // Activated Pillar
        // Skeleton
        // Skeletal Archer

        public RoyalCryptsBot()
        {
            Game.OnTickEvent += OnTick;
            Game.OnDrawEvent += OnDraw;
            Game.OnGameLeaveEvent += OnGameLeave;
        }

        public override void Start()
        {
            if (Me.QuestId != SNOQuestId.SkeletonKing && (Me.QuestStep != 66 || Me.QuestStep != 16) && Me.LevelArea != SNOLevelArea.A1_trDun_Level07B)
            {
                Print("Must start in The Royal Crypts on Reign of the Black King quest, step Find the Crypt of the Skeleton King");
                return;
            }

            Started = true;
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        protected override void OnTick(EventArgs e)
        {
            if (!Started)
                return;

            if (ticks++ % 10 != 0)
                return;

            Unit[] units = Unit.Get();

            // not killable: AnimationId == SNOAnimId.trDun_Crypt_Pillar_idle
            // killable: AnimationId == SNOAnimId.trDun_Crypt_Pillar_Spawner_Spawning
            foreach (var u in units.Where(u => u.Name.Contains("Activated Pillar") && u.ActorId != ActivatedPillar))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            foreach (var u in units.Where(u => u.Name.Contains("Skeleton") && u.ActorId != Skeleton))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            foreach (var u in units.Where(u => u.Name.Contains("Skeletal Archer") && u.ActorId != SkeletalArcher))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            // AnimationId == SNOAnimId.trDun_Crypt_Door_Neutral
            //foreach (var u in units.Where(u => u.Name.Contains("Door") && !Doors.Contains(u.ActorId)))
            //    Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            // SNOActorId.Shrine_Global_Frenzied, no anim change at use wtf
            //foreach (var u in units.Where(u => u.Name.Contains("Shrine")))
            //    Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            foreach (var u in units.Where(u => u.Name == "Stone Coffin" && u.ActorId != StoneCoffin))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            // closed = AnimationId == SNOAnimId.trDun_Floor_Sarcophagus_idle
            foreach (var u in units.Where(u => u.Name == "Sarcophagus" && u.ActorId != Sarcophagus))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            foreach (var u in units.Where(u => u.Name == "Loose Stone" && u.ActorId != LooseStone))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            // closed: AnimationId == SNOAnimId.trDun_Lecturn_idle
            // open: AnimationId == SNOAnimId.trDun_Lecturn_Open
            foreach (var u in units.Where(u => u.Name == "Scribe's Lectern" && u.ActorId != ScribesLectern))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            // closed = AnimationId == SNOAnimId.a1dun_Cath_chest_idle
            // open = AnimationId == SNOAnimId.a1dun_Cath_chest_Open
            foreach (var u in units.Where(u => u.Name == "Chest" && u.ActorId != Chest))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            // closed = AnimationId == SNOAnimId.trDun_Cath_Gate_C_Closed
            // closed = AnimationId == SNOAnimId.trDun_Cath_Gate_C_Open
            foreach (var u in units.Where(u => u.Name.Contains("Gate") && !Gates.Contains(u.ActorId)))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            foreach (var u in units.Where(u => u.Name == "Ashes" && !Ashes.Contains(u.ActorId)))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            foreach (var u in units.Where(u => u.Name == "Dead Villager" && !DeadVillagers.Contains(u.ActorId)))
                Print(String.Format("Unit: {0}, actor {1}, mode {2}, mtype {3}, type {4}, anim {5}", u.Name, u.ActorId, u.Mode, u.MonsterType, u.Type, u.AnimationId));

            //// Kill skeletons
            //var skeletons = units.Where(u => u.ActorId == Skeleton).OrderBy(u => GetDistance(u));
            //foreach (var skeleton in skeletons)
            //{
            //    if (MoveTo(skeleton, RangedDistance))
            //    {
            //        Attack(skeleton);
            //        return;
            //    }
            //}

            //// Kill skeletal archers
            //var skeletalArchers = units.Where(u => u.ActorId == SkeletalArcher).OrderBy(u => GetDistance(u));
            //foreach (var skeleton in skeletalArchers)
            //{
            //    if (MoveTo(skeleton, RangedDistance))
            //    {
            //        Attack(skeleton);
            //        return;
            //    }
            //}

            //if (DestroyAshes(units))
            //    return;

            //// Destroy doors
            //var nearestDoors = units.Where(u => Doors.Contains(u.ActorId));
            //foreach (var door in nearestDoors)
            //{
            //    if (MoveTo(door, RangedDistance))
            //    {
            //        Attack(door);
            //        return;
            //    }
            //}

            // Open closed gates
            var nearestGates = units.Where(u => Gates.Contains(u.ActorId) && CheckCollision(u) && u.AnimationId.ToString().Contains("_Closed"));
            foreach (var gate in nearestGates)
            {
                if (MoveTo(gate))
                {
                    Interact(gate);
                    return;
                }
            }

            //// Open Scribe's Lectern
            //var lecterns = units.Where(u => u.ActorId == ScribesLectern && u.AnimationId == SNOAnimId.trDun_Lecturn_idle);
            //foreach (var lectern in lecterns)
            //{
            //    if (MoveTo(lectern))
            //    {
            //        Interact(lectern);
            //        return;
            //    }
            //}

            //// Open dead villagers
            //var deadvillagers = units.Where(u => DeadVillagers.Contains(u.ActorId) && !OpenedVillagers.Contains(u.ANN));
            //foreach (var villager in deadvillagers)
            //{
            //    if (MoveTo(villager))
            //    {
            //        if (Interact(villager))
            //            OpenedVillagers.Add(villager.ANN);
            //        return;
            //    }
            //}

            var spot = Spots[SpotIndex];

            if (DestroyAshes(units))
                return;

            //if (Me.Mode != UnitMode.Running)
            if (MoveTo(spot.X, spot.Y))
                SpotIndex++;

            if (SpotIndex >= Spots.Count)
                Started = false;
        }

        private bool DestroyAshes(Unit[] units)
        {
            // Destroy ashes
            var nearestAshes = units.Where(u => Ashes.Contains(u.ActorId) && CheckCollision(u) && GetDistance(u) < 35);

            //if (!nearestAshes.Any())
            //    return true;

            foreach (var ash in nearestAshes)
            {
                //if (MoveTo(ash, RangedDistance))
                //{
                    Attack(ash);
                    return true;
                //}
            }

            return false;
        }

        protected override void OnDraw(EventArgs e)
        {
            if (Game.Ingame)
            {
                Draw.DrawText(10.0f, 10.0f, 0x16A, 0x16, 0xFFFFFFFF, String.Format("Position: {0}, {1}, {2}", Me.X, Me.Y, Me.Z));
                Draw.DrawText(10.0f, 30.0f, 0x16A, 0x16, 0xFFFFFFFF, String.Format("LevelArea: {0}", Me.LevelArea));
                Draw.DrawText(10.0f, 50.0f, 0x16A, 0x16, 0xFFFFFFFF, String.Format("Quest: {0}, step {1}", Me.QuestId, Me.QuestStep));

                string unitsText = "Units:\n";

                var units = Unit.Get().Where(u => u.Type != UnitType.Item && CheckCollision(u));

                int index = 0;
                foreach (var unit in units)
                {
                    unitsText += index++ + ": " + unit.Name + " (" + unit.ActorId + ")\n";
                }

                Draw.DrawText(10.0f, 70.0f, 0x16A, 0x16, 0xFFFFFFFF, unitsText);
            }
        }

        protected override void OnGameLeave(EventArgs e)
        {
            OpenedVillagers.Clear();
        }

        private void Attack(Unit unit)
        {
            Me.UsePower(SNOPowerId.DemonHunter_BolaShot, unit);
        }
    }
}
