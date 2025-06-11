global using HelseId.Library.Configuration;
global using HelseId.Library.Interfaces.Caching;
global using HelseId.Library.Interfaces.Endpoints;
global using HelseId.Library.Interfaces.JwtTokens;
global using HelseId.Library.Interfaces.PayloadClaimCreators;
global using HelseId.Library.Interfaces.TokenRequests;
global using HelseId.Library.Services.Caching;
global using HelseId.Library.Services.Endpoints;
global using HelseId.Library.Services.JwtTokens;
global using HelseId.Library.Services.PayloadClaimCreators;
global using HelseId.Library.Services.PayloadClaimCreators.DetailsCreators;
global using HelseId.Library.Services.PayloadClaimCreators.StructuredClaims;
global using HelseId.Library.Services.TokenRequests;
global using HelseId.Library.Models;
global using HelseId.Library.Models.Constants;
global using HelseId.Library.Models.TokenRequests;
global using HelseId.Library.Models.Payloads;
global using HelseId.Library.Models.Configuration;
global using HelseId.Library.ExtensionMethods;
global using HelseId.Library.Exceptions;
global using HelseId.Library.Models.DetailsFromClient;

global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.IdentityModel.JsonWebTokens;
global using Microsoft.IdentityModel.Tokens;

global using System.Net.Http.Json;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Security.Cryptography;
global using System.Security.Cryptography.X509Certificates;

global using HelseId.Library.MachineToMachine;
global using Microsoft.Extensions.DependencyInjection;
