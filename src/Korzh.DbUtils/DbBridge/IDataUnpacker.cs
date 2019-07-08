using System.IO;

namespace Korzh.DbUtils.Packing
{
    public interface IDataUnpacker
    {
        void StartUnpacking();
        void FinishUnpacking();

        bool HasData();

        Stream NextDatasetStream();
    }

}
