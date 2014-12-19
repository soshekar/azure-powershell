﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AutoMapper;
using Microsoft.Azure.Commands.NetworkResourceProvider.Models;
using Microsoft.Azure.Commands.NetworkResourceProvider.Properties;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using MNM = Microsoft.Azure.Management.Network.Models;

namespace Microsoft.Azure.Commands.NetworkResourceProvider
{
    [Cmdlet(VerbsCommon.New, "AzureLoadBalancer"), OutputType(typeof(PSLoadBalancer))]
    public class NewAzureLoadBalancerCmdlet : LoadBalancerBaseClient
    {
        [Alias("ResourceName")]
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The resource name.")]
        [ValidateNotNullOrEmpty]
        public virtual string Name { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public virtual string ResourceGroupName { get; set; }

        [Parameter(
         Mandatory = true,
         ValueFromPipelineByPropertyName = true,
         HelpMessage = "location.")]
        [ValidateNotNullOrEmpty]
        public virtual string Location { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = true,
             HelpMessage = "The list of frontend Ip config")]
        [ValidateNotNullOrEmpty]
        public List<PSFrontendIpConfiguration> FrontendIpConfiguration { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = true,
             HelpMessage = "The list of frontend Ip config")]
        public List<PSBackendAddressPool> BackendAddressPool { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = true,
             HelpMessage = "The list of frontend Ip config")]
        public List<PSProbe> Probe { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = true,
             HelpMessage = "The list of frontend Ip config")]
        public List<PSInboundNatRule> InboundNatRule { get; set; }

        [Parameter(
             Mandatory = false,
             ValueFromPipelineByPropertyName = true,
             HelpMessage = "The list of frontend Ip config")]
        public List<PSLoadBalancingRule> LoadBalancingRule { get; set; }

        [Alias("Tags")]
        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "An array of hashtables which represents resource tags.")]
        public Hashtable[] Tag { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Do not ask for confirmation if you want to overrite a resource")]
        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            if (this.IsLoadBalancerPresent(this.ResourceGroupName, this.Name))
            {
                ConfirmAction(
                    Force.IsPresent,
                    string.Format(Resources.OverwritingResource, Name),
                    Resources.OverwritingResourceMessage,
                    Name,
                    () => CreateLoadBalancer());
            }

            var loadBalancer = this.CreateLoadBalancer();

            WriteObject(loadBalancer);
        }

        private PSLoadBalancer CreateLoadBalancer()
        {
            var loadBalancer = new PSLoadBalancer();
            loadBalancer.Name = this.Name;
            loadBalancer.ResourceGroupName = this.ResourceGroupName;
            loadBalancer.Location = this.Location;

            loadBalancer.Properties = new PSLoadBalancerProperties();
            loadBalancer.Properties.FrontendIpConfigurations = new List<PSFrontendIpConfiguration>();
            loadBalancer.Properties.FrontendIpConfigurations = this.FrontendIpConfiguration;

            if (this.BackendAddressPool.Any())
            {
                loadBalancer.Properties.BackendAddressPools = new List<PSBackendAddressPool>();
                loadBalancer.Properties.BackendAddressPools = this.BackendAddressPool;
            }

            if (this.Probe.Any())
            {
                loadBalancer.Properties.Probes = new List<PSProbe>();
                loadBalancer.Properties.Probes = this.Probe;
            }

            if (this.InboundNatRule.Any())
            {
                loadBalancer.Properties.InboundNatRules = new List<PSInboundNatRule>();
                loadBalancer.Properties.InboundNatRules = this.InboundNatRule;
            }

            if (this.LoadBalancingRule.Any())
            {
                loadBalancer.Properties.LoadBalancingRules = new List<PSLoadBalancingRule>();
                loadBalancer.Properties.LoadBalancingRules = this.LoadBalancingRule;
            }

            // Normalize the IDs
            ChildResourceHelper.NormalizeChildResourcesId(loadBalancer);

            // Map to the sdk object
            var lbModel = Mapper.Map<MNM.LoadBalancerCreateOrUpdateParameters>(loadBalancer);
            lbModel.Tags = TagsConversionHelper.CreateTagDictionary(this.Tag, validate: true);

            // Execute the Create VirtualNetwork call
            this.LoadBalancerClient.CreateOrUpdate(this.ResourceGroupName, this.Name, lbModel);

            var getLoadBalancer = this.GetLoadBalancer(this.ResourceGroupName, this.Name);

            return getLoadBalancer;
        }
    }
}
