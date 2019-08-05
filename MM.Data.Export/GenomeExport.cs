using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace MM.Data.Export
{
    public class GenomeExport
    {
        public GenomeExport() { }

        public void BuildExport(int genomeID)
        {
            var codingGroups = GetCodingGroupIDs(genomeID);
            var effects = new List<Effects>();
            foreach (var cg in codingGroups)
            {
                var cgEffects = GetEffects(cg, genomeID);
                foreach (var eff in cgEffects)
                {
                    effects.Add(eff);
                      
                }
            }

            foreach (var e in effects)
            {
                ProcessActivityGenes(e);
                ProcessOutcomes(e);
            }

            CreateExport(effects);
        }


        public List<DocumentInfo> GetDocuments(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var docs = from g in db.genomedatas
                           join cg in db.coding_groups on g.id equals cg.gdataid
                           where g.genomeid == genomeID
                           select new DocumentCodingGroup
                           {
                               GDataID = g.id,
                               CodingGroupID = cg.id,
                               CreatedBy = cg.createdby
                           };

                var documents = new List<DocumentInfo>();
                foreach (var doc in docs)
                {
                    documents.Add(new DocumentInfo(doc));
                }
                return documents;
            }
        }

        private void ProcessCodingGroupItems(Effects effects)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var codingItems = db.coding_groups_items.Where(_ => _.groupid == effects.CodingGroupID);

                foreach (var item in codingItems)
                {
                    switch (item.objkey)
                    {
                        case "program_bendef":
                            break;
                        case "program_outcome":
                            break;
                        case "program_characteristics":
                            break;
                        case "program_activities":
                            break;
                    }
                }
            }
        }

        //Processing blocks
        public static void ProcessActivityGenes(Effects effects)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                //var activities = db.coding_groups_items.Where(_ => _.gdataid == document.GDataID && _.objkey == "program_activities");
                
                var docGenes = db.coding_activities_ugs.Where(_ => _.gdataid == effects.GDataID);

                foreach (var g in docGenes)
                {
                    var gene = effects.Genes.Where(_ => _.GeneID == g.ugtid).FirstOrDefault();
                    if (gene != null)
                    {
                        gene.IsPresent = true;
                        //Console.WriteLine(gene.GeneID + " | " + gene.IsPresent);
                    }
                }
            }
        }

       
        public static void ProcessOutcomes(Effects effects)
        {
            var max = 0;
            using (GenomeManagerData db = new GenomeManagerData())
            {
                //var codingItems = db.coding_groups_items.Where(_ => _.groupid == effects.CodingGroupID && _.objkey == "program_outcome").FirstOrDefault();

                var codingItems = db.coding_groups_items.Where(_ => _.groupid == effects.CodingGroupID && _.objkey == "program_outcome").ToList();

                if (codingItems.Count() != 0)
                {
                    for (int i = 0; i < codingItems.Count(); ++i)
                    {
                        var objID = codingItems[i].objkeyid;
                        var outcome = db.coding_ns_stem_outcomes.Where(_ => _.id == objID).FirstOrDefault();

                        foreach (var o in effects.Outcomes)
                        {
                            if (outcome.standardized_outcome == o.Name)
                            {
                                o.IsPresent = true;
                            }
                        }

                    }
                }
            }
        }

        public List<long> GetCodingGroupIDs(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var codingGroups = from g in db.genomedatas
                                   join cg in db.coding_groups on g.id equals cg.gdataid
                                   where g.genomeid == genomeID
                                   select cg.id;

                return codingGroups.ToList();
            }
        }

        public List<Effects> GetEffects(long cgID, int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var groupEffects = db.coding_groups_items.Where(_ => _.groupid == cgID && _.objkey == "program_effects").ToList();
                List<Effects> effects = new List<Effects>();

                foreach (var ge in groupEffects)
                {
                    var coding_Program_Effects = db.coding_program_effects.Where(_ => _.id == ge.objkeyid).FirstOrDefault();
                    if (coding_Program_Effects != null)
                    {
                        effects.Add(new Effects(coding_Program_Effects, cgID, genomeID));
                    }

                }
                return effects;
            }
        }


        public static void CreateExport(List<Effects> effects)
        {
            Excel.Application app = new Excel.Application();
            var workBook = app.Workbooks.Add();

            var dictWorkSheet = workBook.Worksheets.Add();

            dictWorkSheet.Name = "Dictionary";

            dictWorkSheet.Cells[1, 1].value = "GeneID";
            dictWorkSheet.Cells[1, 2].value = "Gene Name";

            dictWorkSheet.Cells[1, 4].value = "OutComeID";
            dictWorkSheet.Cells[1, 5].value = "Outome Name";
            dictWorkSheet.Cells[1, 6].value = "Outome Description";

            var geneCount = 0;
            for (int row = 2; row < effects[0].Genes.Count + 2; row++)
            {
                dictWorkSheet.Cells[row, 1].value = effects[0].Genes[geneCount].GeneID;
                dictWorkSheet.Cells[row, 2].value = effects[0].Genes[geneCount].Name;
                geneCount++;
            }

            var oucomeCount = 0;
            for (int row = 2; row < effects[0].Outcomes.Count() + 2; row++)
            {
                dictWorkSheet.Cells[row, 4].value = effects[0].Outcomes[oucomeCount].OutcomeID;
                dictWorkSheet.Cells[row, 5].value = effects[0].Outcomes[oucomeCount].Name;
                dictWorkSheet.Cells[row, 6].value = effects[0].Outcomes[oucomeCount].Description;
                oucomeCount++;
            }
                

            var workSheet = workBook.Worksheets.Add();
            workSheet.Name = "DataExport";
            workSheet.Cells[1, 1].value = "GDataID";
            workSheet.Cells[1, 2].value = "Created By"; 
            workSheet.Cells[1, 3].value = "Effect ID";
            workSheet.Cells[1, 4].value = "Effect Name";
            workSheet.Cells[1, 5].value = "Effect Description";
            workSheet.Cells[1, 6].value = "IV Name',";
            workSheet.Cells[1, 7].value = "DV Name";
            workSheet.Cells[1, 8].value = "Who is the DV";
            workSheet.Cells[1, 9].value = "DV Delay";
            workSheet.Cells[1, 10].value = "Randimization Scheme";
            workSheet.Cells[1, 11].value = "Random Units";
            workSheet.Cells[1, 12].value = "Analysis level";
            workSheet.Cells[1, 13].value = "Control Description";
            workSheet.Cells[1, 14].value = "Cluster Info";
            workSheet.Cells[1, 15].value = "# of control clusters";
            workSheet.Cells[1, 16].value = "# of treatment clusters";
            workSheet.Cells[1, 17].value = "Subject info";
            workSheet.Cells[1, 18].value = "# control subjects";
            workSheet.Cells[1, 19].value = "Total attrition";
            workSheet.Cells[1, 20].value = "Diff attrition";
            workSheet.Cells[1, 21].value = "Used inst. var";
            workSheet.Cells[1, 22].value = "Used control var";
            workSheet.Cells[1, 23].value = "Used reg dis";
            workSheet.Cells[1, 24].value = "Used matching";
            workSheet.Cells[1, 25].value = "Baseline";
            workSheet.Cells[1, 26].value = "Claimed reliablity";
            workSheet.Cells[1, 27].value = "pvalue";
            workSheet.Cells[1, 28].value = "SMD Desc";
            workSheet.Cells[1, 29].value = "SMD";
            workSheet.Cells[1, 30].value = "Est. SMD";
            workSheet.Cells[1, 31].value = "Standard error SMD";
            workSheet.Cells[1, 32].value = "p provided";
            workSheet.Cells[1, 33].value = "Control type";

            geneCount = 0;
            for (int col = 34; col <= effects[0].Genes.Count; col++)
            {
                workSheet.Cells[1, col].value = "GeneID: " + effects[0].Genes[geneCount].GeneID;
                geneCount++;

            }

            var outcomeCount = 0;
            var outcomeStart = geneCount + 34;
            for (int x = 0; x <= 3; x++)
            {
                outcomeCount = 0;
                //for (int col = 0; col < effects[0].Outcomes.Count; col++)
                //for (int col = 0; col < effects[0].Outcomes.Count; col++)
                //{
                    workSheet.Cells[1, outcomeStart].value = "OutcomeID: " + effects[0].Outcomes[x].OutcomeID;
                    outcomeCount++;
                    outcomeStart++;
                //}
            }


            var docCount = 0;
            var rowCount = 2;
            var columnCount = effects[0].Genes.Count() + 2;
            var test = effects.Take(20);
            foreach (var eff in effects)
            {
                workSheet.Cells[rowCount, 1].value = eff.GDataID;
                workSheet.Cells[rowCount, 2].value = eff.CreatedBy;
                workSheet.Cells[rowCount, 3].value = eff.EffectsID;
                workSheet.Cells[rowCount, 4].value = eff.Name;
                workSheet.Cells[rowCount, 5].value = eff.Description;
                workSheet.Cells[rowCount, 6].value = eff.IVName;
                workSheet.Cells[rowCount, 7].value = eff.DVName;
                workSheet.Cells[rowCount, 8].value = eff.DVWho;
                workSheet.Cells[rowCount, 9].value = eff.DVDelay;
                workSheet.Cells[rowCount, 10].value = eff.RandomScheme;
                workSheet.Cells[rowCount, 11].value = eff.RandomUnits;
                workSheet.Cells[rowCount, 12].value = eff.AnalysisLevel;
                workSheet.Cells[rowCount, 13].value = eff.ControlDescription;
                workSheet.Cells[rowCount, 14].value = eff.ClusterInfo;
                workSheet.Cells[rowCount, 15].value = eff.NumberOfControlClusters;
                workSheet.Cells[rowCount, 16].value = eff.NumberOfTreatmentClusters;
                workSheet.Cells[rowCount, 17].value = eff.SubjectInfo;
                workSheet.Cells[rowCount, 18].value = eff.NumberOfControlSubjects;
                workSheet.Cells[rowCount, 19].value = eff.TotalAttrition;
                workSheet.Cells[rowCount, 20].value = eff.DiffAttrition;
                workSheet.Cells[rowCount, 21].value = eff.UsedInstVar;
                workSheet.Cells[rowCount, 22].value = eff.UsedControlVar;
                workSheet.Cells[rowCount, 23].value = eff.UsedRegDis;
                workSheet.Cells[rowCount, 24].value = eff.UsedMatching;
                workSheet.Cells[rowCount, 25].value = eff.Baseline;
                workSheet.Cells[rowCount, 26].value = eff.ClaimedReliability;
                workSheet.Cells[rowCount, 27].value = eff.PValue;
                workSheet.Cells[rowCount, 28].value = eff.SMDDesc;
                workSheet.Cells[rowCount, 29].value = eff.SMD;
                workSheet.Cells[rowCount, 30].value = eff.EstSMD;
                workSheet.Cells[rowCount, 31].value = eff.StandartErrorSMD;
                workSheet.Cells[rowCount, 32].value = eff.PProvided;
                workSheet.Cells[rowCount, 33].value = eff.ControlType;


                geneCount = 0;
                for (int column = 34; column <= effects[0].Genes.Count; column++)
                {
                    workSheet.Cells[rowCount, column].value = eff.Genes[geneCount].IsPresent == false ? 0 : 1;
                    geneCount++;
                }


                outcomeCount = geneCount + 34;
                outcomeStart = 0;
                for (int i = 0; i < effects[0].Outcomes.Count; i++)
                {
                    workSheet.Cells[rowCount, outcomeCount].value = eff.Outcomes[outcomeStart].IsPresent == false ? 0 : 1;
                    outcomeStart++;
                    outcomeCount++;
                    
                }

                     //    outcomeStart = 0;
                    //    foreach (var outcome in eff.EffectsOutcomes)
                    //    {

                    //        workSheet.Cells[rowCount, outcomeCount].value = outcome[outcomeStart].IsPresent == false ? 0 : 1;
                    //        outcomeStart++;
                    //        outcomeCount++;
                    //    }

                rowCount++;

            }

            var time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            workBook.SaveAs(@"c:\temp\export_" + time + ".xlsx");
            workBook.Close();
            app.Quit();

        }


    }
}
