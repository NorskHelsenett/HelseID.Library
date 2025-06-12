global using HelseId.Library.Configuration;
global using HelseId.Library.Interfaces.Caching;
global using HelseId.Library.Interfaces.Endpoints;
global using HelseId.Library.Interfaces.JwtTokens;
global using HelseId.Library.Interfaces.PayloadClaimCreators;
global using HelseId.Library.Interfaces.TokenRequests;
global using HelseId.Library.Services.TokenRequests;
global using HelseId.Library.Models;
global using HelseId.Library.Models.Constants;
global using HelseId.Library.Models.TokenRequests;
global using HelseId.Library.Models.Payloads;
global using HelseId.Library.Models.Configuration;
global using HelseId.Library.Models.DetailsFromClient;
global using HelseId.Library.ExtensionMethods;

global using HelseId.Library.MachineToMachine.Interfaces;
global using HelseId.Library.MachineToMachine.Interfaces.TokenRequests;
global using HelseId.Library.MachineToMachine.Interfaces.PayloadClaimCreators;
global using HelseId.Library.MachineToMachine.Models.TokenRequests;
global using HelseId.Library.MachineToMachine.PayloadClaimCreators;
global using HelseId.Library.MachineToMachine.Services.TokenRequests;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.IdentityModel.JsonWebTokens;
global using System.Net.Http.Json;
global using System.Text.Json;

using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("HelseId.Library.MachineToMachine.Tests")]
