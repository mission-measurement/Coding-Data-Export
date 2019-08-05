using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Data.Export
{
    public class Effects
    {
        public long GDataID { get; set; }
        public long CreatedBy { get; set; }
        public long EffectsID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CodingGroupID { get; set; }
        public int GenomeID { get; set; }
        public string IVName { get; set; }
        public string DVName { get; set; }
        public string DVWho { get; set; }
        public string DVDelay { get; set; }
        public string RandomScheme { get; set; }
        public string RandomUnits { get; set; }
        public string AnalysisLevel { get; set; }
        public string ControlDescription { get; set; }
        public string ClusterInfo { get; set; }
        public string NumberOfControlClusters { get; set; }
        public string NumberOfTreatmentClusters { get; set; }
        public string SubjectInfo { get; set; }
        public string NumberOfControlSubjects { get; set; }
        public string TotalAttrition { get; set; }
        public string DiffAttrition { get; set; }
        public string UsedInstVar { get; set; }
        public string UsedControlVar { get; set; }
        public string UsedRegDis { get; set; }
        public string UsedMatching { get; set; }
        public string Baseline { get; set; }
        public string ClaimedReliability { get; set; }
        public string PValue { get; set; }
        public string SMDDesc { get; set; }
        public string SMD { get; set; }
        public string EstSMD { get; set; }
        public string StandartErrorSMD { get; set; }
        public string PProvided { get; set; }
        public string ControlType { get; set; }

        public List<List<Outcome>> EffectsOutcomes { get; set; }

        public List<Outcome> Outcomes { get; set; }
        public List<Gene> Genes { get; set; }
        //public List<Beneficiary> Beneficiaries { get; set; }
        //public List<Characteristic> Characteristics { get; set; }

        public Effects() { }

        public Effects(coding_program_effects effect, long codingGroupID, int genomeID)
        {
            GDataID = effect.gdataid;
            CreatedBy = effect.createdby;
            Description = effect.pe_sampledesc;
            EffectsID = effect.id;
            CodingGroupID = codingGroupID;
            GenomeID = genomeID;
            IVName = effect.pe_ivshortname;
            DVName = effect.pe_dvshortname;
            DVWho = effect.pe_dvwho;
            DVDelay = effect.pe_dvdelay;
            RandomScheme = effect.pe_randscheme;
            RandomUnits = effect.pe_randunits == null ? String.Empty : effect.pe_randunits.ToString();
            AnalysisLevel = effect.pe_analysislevel;
            ControlDescription = effect.pe_cntrldesc;
            ClusterInfo = effect.pe_clusterinfo;
            NumberOfTreatmentClusters = effect.pe_numtreatclus == null ? String.Empty : effect.pe_numtreatclus.ToString();
            SubjectInfo = effect.pe_subjectinfo;
            NumberOfControlSubjects = effect.pe_numsubcntrl == null ? String.Empty : effect.pe_numsubcntrl.ToString();
            TotalAttrition = effect.pe_totattrition == null ? String.Empty : effect.pe_totattrition.ToString();
            DiffAttrition = effect.pe_diffattrition == null ? String.Empty : effect.pe_diffattrition.ToString();
            UsedControlVar = effect.controlvars_used == null ? String.Empty : effect.controlvars_used.ToString();
            UsedRegDis = effect.pe_usedregdis;
            UsedMatching = effect.pe_usedmatching;
            Baseline = effect.pe_baseline;
            ClaimedReliability = effect.pe_claimrelia;
            PValue = effect.pe_pvalue;
            SMDDesc = effect.pe_smddesc;
            SMD = effect.pe_smd;
            EstSMD = effect.pe_estsmd == null ? String.Empty : effect.pe_estsmd.ToString(); 
            StandartErrorSMD = effect.pe_sesmd == null ? String.Empty : effect.pe_sesmd.ToString(); 
            PProvided = effect.pe_pprovided;
            ControlType = effect.pe_cntrltype;
            //additions
            Genes = GetGeneList(genomeID);
            Outcomes = GetOutcomesList(genomeID);
            EffectsOutcomes = GetEffectsOutcomes(genomeID);
            //Beneficiaries = GetBeneficiariesList(genomeID);
            //Characteristics = GetCharacteristicsList(genomeID);
        }


        #region Beneficiaries

        public List<Beneficiary> GetBeneficiariesList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var gdataList = db.genomedatas.Where(_ => _.genomeid == genomeID).ToList();
                var allben = db.coding_ns_stem_bendef.ToList();

                var benList = (from b in allben
                               join g in gdataList on b.gdataid equals g.id
                              select b).ToList();

                var distinctBen = benList.GroupBy(x => x.name).Select(x => x.FirstOrDefault());
                var beneficiaryList = new List<Beneficiary>();

                var count = 0;
                foreach (var ben in distinctBen)
                {
                    var beneficiary = new Beneficiary();
                    beneficiary.BeneficiaryID = count;
                    beneficiary.Name = ben.name;
                    beneficiaryList.Add(beneficiary);
                    count++;
                }
                return beneficiaryList;
            }
        }

        #endregion

        #region Characteristics
        public List<Characteristic> GetCharacteristicsList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var gdataList = db.genomedatas.Where(_ => _.genomeid == genomeID).ToList();

                var allChar = db.coding_program_characteristics.ToList();

                var charList = (from c in allChar
                                join g in gdataList on c.gdataid equals g.id
                                select c).ToList();

                var distinctChar = charList.GroupBy(x => x.name).Select(x => x.FirstOrDefault());
                var charateristicsList = new List<Characteristic>();

                var count = 0;
                foreach(var c in distinctChar)
                {
                    var characteristic = new Characteristic();
                    characteristic.CharacteristicID = count;
                    characteristic.Name = c.name;
                    charateristicsList.Add(characteristic);
                }

                return charateristicsList;
            }
        }


        #endregion


        public List<List<Outcome>> GetEffectsOutcomes(int genomeID)
        {
            var outcomes = new List<List<Outcome>>();
            for (int x = 0; x <= 3; x++)
            {
                outcomes.Add(GetOutcomesList(genomeID));
            }

            return outcomes;
        }

        #region Outcomes 
        public List<Outcome> GetOutcomesList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var uOutcomesList = db.master_outcome_list.Where(_ => _.genomeid == genomeID).ToList();
                var outcomes = new List<Outcome>();

                foreach (var item in uOutcomesList)
                {
                    //ignore dead outcomes
                    if (item.outcomeid != 51 && item.outcomeid != 52)
                    {
                        var outcome = new Outcome();
                        outcome.OutcomeID = item.outcomeid;
                        outcome.Name = item.std_outcome_name;
                        
                        outcomes.Add(outcome);
                    }
                }
                return outcomes;
            }
        }


        #endregion

        #region Genes
        public List<Gene> GetGeneList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var uGenelist = db.master_universalgene_types.Where(_ => _.genomeid == genomeID).ToList();

                var genes = new List<Gene>();
                foreach (var g in uGenelist)
                {
                    var gene = new Gene();
                    gene.GeneID = g.id;
                    gene.Name = g.ugtname;
                    gene.GenomeID = g.genomeid;
                    genes.Add(gene);
                }
                return genes;
            }
        }

        #endregion
    }
}
