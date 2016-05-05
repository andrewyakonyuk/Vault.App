using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vault.Framework.Models.Overrides
{
    public class MusicRecordingMapping : IAutoMappingOverride<MusicRecording>
    {
        public void Override(AutoMapping<MusicRecording> mapping)
        {
            mapping.References(t => t.ByArtist).Cascade.SaveUpdate();
            mapping.References(t => t.InAlbum).Cascade.SaveUpdate();
        }
    }
}