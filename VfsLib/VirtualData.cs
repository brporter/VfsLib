using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace BryanPorter.Web.VirtualFS
{
    public class VirtualData
        : IVFSData
    {
        private readonly HttpServerUtilityBase _utility;
        private readonly string _connectionString;

        public VirtualData(HttpServerUtilityBase utility, string connectionString)
            : this(utility, connectionString, null)
        {
        }

        public VirtualData(HttpServerUtilityBase utility, string connectionString, IVFSData parent)
        {
            _connectionString = connectionString;
            _utility = utility;

            ParentRepository = parent ?? this;
        }

        public IVFSData ParentRepository
        {
            get;
            set;
        }

        public bool FileExists(string virtualPath)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "dbo.FileExists";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var fileName = System.IO.Path.GetFileName(virtualPath);
                    var dirPathOnly = System.IO.Path.GetDirectoryName(virtualPath);

                    cmd.Parameters.AddWithValue("@FolderPath", CorrectPath(dirPathOnly, true));
                    cmd.Parameters.AddWithValue("@FileName", fileName);

                    cn.Open();
                    var result = cmd.ExecuteScalar();

                    if (result is bool)
                        return (bool)result;

                    // TODO: replace with more appropriate exception type
                    throw new InvalidOperationException();
                }
            }
        }

        public bool DirectoryExists(string virtualPath)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "dbo.DirectoryExists";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DirectoryPath", CorrectPath(virtualPath, true));

                    cn.Open();
                    var result = cmd.ExecuteScalar();

                    if (result is bool)
                    {
                        return (bool)result;
                    }

                    // TODO: replace with more appropriate exception type
                    throw new InvalidOperationException();
                }
            }
        }

        private static IDataReader GetChildren(string connectionString, string virtualPath, bool includeFiles, bool includeDirectories)
        {
            var cn = new SqlConnection(connectionString);
            IDataReader result = null;

            try
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "dbo.GetChildren";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DirectoryPath", CorrectPath(virtualPath, true));
                    cmd.Parameters.AddWithValue("@IncludeFiles", includeFiles);
                    cmd.Parameters.AddWithValue("@IncludeDirectories", includeDirectories);

                    cn.Open();
                    result = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch
            {
                if (cn != null)
                    cn.Dispose();
            }

            return result;
        }

        public IEnumerable<System.Web.Hosting.VirtualFileBase> GetChildren(string virtualPath)
        {
            using (var dr = GetChildren(_connectionString, virtualPath, true, true))
            {
                // Directories
                while (dr.Read())
                {
                    var path = dr.GetString(2);

                    if (string.Compare(path, virtualPath, true, System.Globalization.CultureInfo.InvariantCulture) == 0) // Don't return ourselves in our list of children.
                        continue;

                    yield return new VirtualFileSystemDirectory(dr.GetString(2), this, _utility);
                }

                dr.NextResult();

                // Files
                while (dr.Read())
                {
                    yield return new VirtualFileSystemFile(dr.GetString(4), this, _utility);
                }
            }
        }

        public IEnumerable<VirtualFileSystemFile> GetFiles(string virtualPath)
        {
            using (var dr = GetChildren(_connectionString, virtualPath, true, false))
            {
                // Files
                while (dr.Read())
                    yield return new VirtualFileSystemFile(dr.GetString(4), this, _utility);
            }
        }

        public IEnumerable<VirtualFileSystemDirectory> GetDirectories(string virtualPath)
        {
            using (var dr = GetChildren(_connectionString, virtualPath, true, true))
            {
                // Directories
                while (dr.Read())
                {
                    var path = dr.GetString(2);

                    if (string.Compare(path, virtualPath, true, System.Globalization.CultureInfo.InvariantCulture) == 0) // Don't return ourselves in our list of children.
                        continue;

                    yield return new VirtualFileSystemDirectory(dr.GetString(2), this, _utility);
                }
            }
        }

        public byte[] GetFileContents(string virtualPath)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "dbo.GetFileContents";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var fileName = System.IO.Path.GetFileName(virtualPath);
                    var dirPathOnly = System.IO.Path.GetDirectoryName(virtualPath);

                    cmd.Parameters.AddWithValue("@FolderPath", CorrectPath(dirPathOnly, true));
                    cmd.Parameters.AddWithValue("@FileName", fileName);

                    cn.Open();
                    var result = cmd.ExecuteScalar();

                    var bytes = result as byte[];

                    return bytes;
                }
            }
        }

        public VirtualFileSystemDirectory GetDirectory(string virtualPath)
        {
            if (DirectoryExists(virtualPath))
                return new VirtualFileSystemDirectory(virtualPath, this, _utility);

            return null;
        }

        public VirtualFileSystemFile GetFile(string virtualPath)
        {
            if (FileExists(virtualPath))
                return new VirtualFileSystemFile(virtualPath, this, _utility);

            return null;
        }

        private static string CorrectPath(string virtualPath, bool isDir)
        {
            // Virtual Path Fixup - DAL expects virtual paths to use foreach slash seperator, and
            // for directory specs' always end in a slash.

            var correctedPath = new StringBuilder(virtualPath);

            correctedPath.Replace("\\", "/");

            if (isDir && !virtualPath.EndsWith("/"))
                correctedPath.Append("/");

            return correctedPath.ToString();
        }
    }
}
