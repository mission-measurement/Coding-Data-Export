using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Data.Export
{
    public class DocumentInfo
    {
        public long GDataID { get; set; }
        public string DocumentName { get; set; }
        public long CreatedBy { get; set; }
        public long CodingGroupID { get; set; }
        public long GenomeID { get; set; }

        public List<Gene> Genes { get; set; }

        public DocumentInfo() { }

        public DocumentInfo(DocumentCodingGroup documentCodingGroup)
        {
            GDataID = documentCodingGroup.GDataID;
            var doc = GetDocumentInfo(documentCodingGroup.GDataID);
            DocumentName = doc.documenttitle;
            CreatedBy = documentCodingGroup.CreatedBy;
            CodingGroupID = documentCodingGroup.CodingGroupID;
            Genes = BuildGeneList((int)doc.genomeid);
        }


        private genomedata GetDocumentInfo(long gDataID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var doc = db.genomedatas.Where(_ => _.id == gDataID).FirstOrDefault();
                return doc;
            }
        }

        private List<Gene> BuildGeneList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var uGenelist = GetGeneList(genomeID);
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

        public List<master_universalgene_types> GetGeneList(int genomeID)
        {
            using (GenomeManagerData db = new GenomeManagerData())
            {
                var uGenes = db.master_universalgene_types.Where(_ => _.genomeid == genomeID).ToList();

                return uGenes;
            }
        }


    }
}
