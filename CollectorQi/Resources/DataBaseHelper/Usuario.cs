namespace CollectorQi.Resources.DataBaseHelper
{
    public class Usuario 
    {

        /*
        private DataBase db;
        public Usuario()
        {
            db = new DataBase();
        }

        public bool InserirUsuario(UsuarioVO byUsuario)
        {
            try
            {
                    db.get().Insert(byUsuario);
                    return true;
                
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLite Exception", ex.Message);
                return false;
            }
        }
        public List<UsuarioVO> GetUsuarios()
        {
            try
            {
                    return db.get().Table<UsuarioVO>().ToList();
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLite Exception", ex.Message);
                return null;
            }
        }
        public bool AtualizarUsuarioId(UsuarioVO byUsuario)
        {
            try
            {
                    db.get().Query<UsuarioVO>("UPDATE UsuarioVO set CodUsuario=?, CodSenha=?, Ativo=?, Tipo=?, Administrador=?, Versao=? Where UsuarioId=?", byUsuario.CodUsuario, byUsuario.CodSenha, byUsuario.Ativo, byUsuario.Tipo, byUsuario.Administrador, byUsuario.Versao, byUsuario.UsuarioId);
                    return true;
             
            }

            catch (SQLiteException ex)
            {
                Log.Warning("SQLite Exception", ex.Message);
                return false;
            }
        }
        public bool DeletarUsuario(UsuarioVO byUsuario)
        {
            try
            {
                
                    db.get().Delete(byUsuario);
                    return true;
                
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLite Exception", ex.Message);
                return false;
            }
        }
        public UsuarioVO GetUsuario(int byUsuarioId)
        {
            UsuarioVO usuario = new UsuarioVO();
            try
            {
                    usuario = db.get().Table<UsuarioVO>().FirstOrDefault(UsuarioVO => UsuarioVO.UsuarioId == byUsuarioId);
                
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLite Exception", ex.Message);
            }
            return usuario;
        }
        public UsuarioVO GetUsuario(string byCodUsuario, string byCodSenha)
        {
            UsuarioVO usuario = new UsuarioVO();
            try
            {
                    usuario = db.get().Table<UsuarioVO>().FirstOrDefault(UsuarioVO => UsuarioVO.CodUsuario == byCodUsuario && UsuarioVO.CodSenha == byCodSenha);
             
            }
            catch (SQLiteException ex)
            {
                Log.Warning("SQLite Exception", ex.Message);
            }
            return usuario;
        }*/

    }
}
