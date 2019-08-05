using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace MM.Data.Export
{
    public class WorkforceExport
    {
        public WorkforceExport() { }

        private static List<DocumentInfo> GetDocuments(long geneomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var docs = from g in db.genomedatas
                           join cg in db.coding_groups on g.id equals cg.gdataid
                           where g.genomeid == geneomeID
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


        public static void ProcessDocumentGenes(DocumentInfo document)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                //var activities = db.coding_groups_items.Where(_ => _.gdataid == document.GDataID && _.objkey == "program_activities");


                var docGenes = db.coding_activities_ugs.Where(_ => _.gdataid == document.GDataID);

                foreach (var g in docGenes)
                {
                    var gene = document.Genes.Where(_ => _.GeneID == g.ugtid).FirstOrDefault();
                    if (gene != null)
                    {
                        gene.IsPresent = true;
                        Console.WriteLine(gene.GeneID + " | " + gene.IsPresent);
                    }
                }
            }
        }

        public static void ProcessDocumentActivitiesGenes(DocumentInfo document)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var activities = db.coding_groups_items.Where(_ => _.gdataid == document.GDataID && _.objkey == "program_activities");



                var docGenes = db.coding_activities_ugs.Where(_ => _.gdataid == document.GDataID);

                foreach (var g in docGenes)
                {
                    var gene = document.Genes.Where(_ => _.GeneID == g.ugtid).FirstOrDefault();
                    if (gene != null)
                    {
                        gene.IsPresent = true;
                        Console.WriteLine(gene.GeneID + " | " + gene.IsPresent);
                    }
                }
            }
        }

        public static void CreateExport(List<DocumentInfo> docs)
        {
            Excel.Application app = new Excel.Application();
            var workBook = app.Workbooks.Add();

            var dictWorkSheet = workBook.Worksheets.Add();

            dictWorkSheet.Name = "Dictionary";

            dictWorkSheet.Cells[1, 1].value = "GeneID";
            dictWorkSheet.Cells[1, 2].value = "Gene Name";

            var geneCount = 0;
            for (int row = 2; row < docs[0].Genes.Count; row++)
            {
                dictWorkSheet.Cells[row, 1].value = docs[0].Genes[geneCount].GeneID;
                dictWorkSheet.Cells[row, 2].value = docs[0].Genes[geneCount].Name;
                geneCount++;
            }

            var workSheet = workBook.Worksheets.Add();
            workSheet.Name = "DataExport";
            workSheet.Cells[1, 1].value = "GDataID";
            workSheet.Cells[1, 2].value = "CreatedBy";
            workSheet.Cells[1, 3].value = "CodingGroupID";
            geneCount = 0;
            for (int col = 4; col <= docs[0].Genes.Count; col++)
            {
                workSheet.Cells[1, col].value = "GeneID: " + docs[0].Genes[geneCount].GeneID;
                geneCount++;

            }

            var docCount = 0;
            var rowCount = 2;
            var columnCount = docs[0].Genes.Count() + 2;
            foreach (var doc in docs)
            {
                workSheet.Cells[rowCount, 1].value = doc.GDataID;
                workSheet.Cells[rowCount, 2].value = doc.CreatedBy;
                workSheet.Cells[rowCount, 3].value = doc.CodingGroupID;
                geneCount = 0;
                for (int column = 4; column < docs[0].Genes.Count; column++)
                {
                    workSheet.Cells[rowCount, column].value = doc.Genes[geneCount].IsPresent == false ? 0 : 1;
                    geneCount++;
                }
                rowCount++;
            }

            var time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            workBook.SaveAs(@"c:\temp\export_" + time + ".xlsx");
            workBook.Close();
            app.Quit();

        }





        private static void ProcessCodingGroups(long gDataID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var groups = db.coding_groups.Where(_ => _.gdataid == gDataID).Select(x => x.id);

                foreach (var groupID in groups)
                {
                    //ProcessCodingGroupItems(groupID);
                }
            }
        }



        private static void GetActivities(long objKeyID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var activities = db.coding_program_activities.Where(_ => _.id == objKeyID).FirstOrDefault();

                var gene = db.coding_activities_ugs.Where(_ => _.pactid == activities.id).FirstOrDefault();


            }
        }

        public static void GetAllGenes(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var genes = db.master_universalgene_types.Where(_ => _.genomeid == genomeID);

                foreach (var g in genes)
                {
                    Console.WriteLine(g.ugtname);
                }
            }


        }
        public static void GetGeneList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var distinctActivities = from cpa in db.coding_program_activities
                                         join cgi in db.coding_groups_items on cpa.id equals cgi.objkeyid
                                         join cg in db.coding_groups on cgi.groupid equals cg.id
                                         join g in db.genomedatas on cg.gdataid equals g.id
                                         where g.genomeid == genomeID
                                         select cpa;

                foreach (var a in distinctActivities)
                {

                    Console.WriteLine(a.name);
                }

            }
        }

    }
}
