using System;
using System.Collections.Generic;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Common;
using FamilyTreeProject.Data.Common;
using FamilyTreeProject.DomainServices;
using FamilyTreeProject.DomainServices.Common;
using FamilyTreeProject.GEDCOM;

namespace FamilyTreeProject.Data.Converters
{
    public class GEDCOMConverter
    {
        private readonly IFamilyTreeServiceFactory _serviceFactory;
        private readonly Dictionary<int, string> _individualLookup = new Dictionary<int, string>();
        private readonly Dictionary<int, string> _sourceLookup = new Dictionary<int, string>();
        private readonly Dictionary<int, string> _repositoryLookup = new Dictionary<int, string>();

        public GEDCOMConverter(IFamilyTreeServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public string Import(IFileStore store, Tree tree, bool updateIds)
        {
            var treeService = _serviceFactory.CreateTreeService();
            treeService.Add(tree);

            //Add Repositories
            ProcessRepositories(store.Repositories, tree.UniqueId, updateIds);

            //Add Sources0
            ProcessSources(store.Sources, tree.UniqueId, updateIds);

            //Add Individuals
            ProcessIndividuals(store.Individuals, tree.UniqueId, updateIds);

            //Add Families
            ProcessFamilies(store.Families, tree.UniqueId, updateIds);


            return tree.UniqueId;
        }
        
        private void ProcessFamilies(IList<Family> families, string treeId, bool updateIds)
        {
            var familyService = _serviceFactory.CreateFamilyService();

            foreach (var family in families)
            {
                family.TreeId = treeId;
                family.Id = 0;               

                family.HusbandId = GetReferenceId(family.HusbandId, _individualLookup);
                family.WifeId = GetReferenceId(family.WifeId, _individualLookup);

                if (updateIds)
                {
                    foreach (var citation in family.Citations)
                    {
                        UpdateCitationSource(citation);
                    }
                    foreach (var fact in family.Facts)
                    {
                        foreach (var citation in fact.Citations)
                        {
                            UpdateCitationSource(citation);
                        }
                    }
                }
                familyService.Add(family);
            }
        }

        private void ProcessIndividuals(IList<Individual> individuals, string treeId, bool updateIds)
        {
            var individualService = _serviceFactory.CreateIndividualService();

            foreach (var individual in individuals)
            {
                individual.TreeId = treeId;
                if (updateIds)
                {
                    foreach (var citation in individual.Citations)
                    {
                        UpdateCitationSource(citation);
                    }
                    foreach (var fact in individual.Facts)
                    {
                        foreach (var citation in fact.Citations)
                        {
                            UpdateCitationSource(citation);
                        }
                    }
                }
                AddEntity(_individualLookup, individual, individualService, updateIds);
            }

            //Fix ancestor references
            if (updateIds)
            {
                foreach (var individual in individuals)
                {
                    individual.FatherId = GetReferenceId(individual.FatherId, _individualLookup);
                    individual.MotherId = GetReferenceId(individual.MotherId, _individualLookup);

                    individualService.Update(individual);
                }
            }
        }

        private void UpdateCitationSource(Citation citation)
        {
            citation.SourceId = GetReferenceId(citation.SourceId, _sourceLookup);
        }

        private void ProcessRepositories(IList<Repository> repositories, string treeId, bool updateIds)
        {
            var repositoryService = _serviceFactory.CreateRepositoryService();

            foreach (var repository in repositories)
            {
                repository.TreeId = treeId;

                AddEntity(_repositoryLookup, repository, repositoryService, updateIds);
            }
        }

        private void ProcessSources(IList<Source> sources, string treeId, bool updateIds)
        {
            var sourceService = _serviceFactory.CreateSourceService();

            foreach (var source in sources)
            {
                source.TreeId = treeId;
                if (updateIds)
                {
                    source.RepositoryId = GetReferenceId(source.RepositoryId, _repositoryLookup);
                }

                AddEntity(_sourceLookup, source, sourceService, updateIds);
            }
        }

        private void AddEntity<T>(Dictionary<int, string> lookup, T entity, IEntityService<T> service, bool updateIds) where T: Entity
        {
            int originalId = 0;

            if (updateIds)
            {
                originalId = entity.Id;
                entity.Id = 0;
            }
                
            service.Add(entity);
            if (updateIds)
            {
                lookup[originalId] = entity.UniqueId;
            }
        }

        private string GetReferenceId(string sourceId, Dictionary<int, string> lookup)
        {
            string updatedId = String.Empty;
            
            if (!string.IsNullOrEmpty(sourceId))
            {
                var referenceId = GEDCOMUtil.GetId(sourceId);
                if (referenceId.HasValue)
                {
                    updatedId = lookup[referenceId.Value];
                }
            }

            return updatedId;
        }
    }
}