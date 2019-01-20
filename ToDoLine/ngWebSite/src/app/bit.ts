/// <reference path="../../node_modules/@bit/bit-framework/bit.core.d.ts" />
/// <reference path="../../node_modules/@bit/bit-framework/bit.model.context.d.ts" />
/// <reference path="../../node_modules/@bit/bit-framework/bit.core.declarations.d.ts" />
/// <reference path="../../node_modules/@bit/jaydata/jaydata.d.ts" />

export const SecurityService = new Bit.Implementations.DefaultSecurityService();
export const MessageReciever: Bit.Contracts.IMessageReceiver = new Bit.Implementations.SignalRMessageReceiver();
export const MetadataProvider: Bit.Contracts.IMetadataProvider = new Bit.Implementations.DefaultMetadataProvider();
export const GuidUtils = new Bit.Implementations.DefaultGuidUtils();
export const EntityContextProvider: Bit.Contracts.IEntityContextProvider =
    new Bit.Implementations.EntityContextProviderBase(GuidUtils, MetadataProvider, SecurityService);
export const DateTimeService: Bit.Contracts.IDateTimeService = new Bit.Implementations.DefaultDateTimeService();
export const MathService: Bit.Contracts.IMath = new Bit.Implementations.DefaultMath();
export const SyncService: Bit.Contracts.ISyncService = new Bit.Implementations.DefaultSyncService();
export const ClientAppProfile = Bit.ClientAppProfileManager.getCurrent();
export const Logger: Bit.Contracts.ILogger = new Bit.Implementations.LoggerBase(EntityContextProvider, ClientAppProfile);

Bit.Provider.loggerProvider = () => Logger;

Bit.Provider.getFormattedDateDelegate = (date: Date, culture: string): string => {
  throw new Error('NotImplemented');
};

Bit.Provider.getFormattedDateTimeDelegate = (date: Date, culture: string): string => {
  throw new Error('NotImplemented');
};

Bit.Provider.parseDateDelegate = (date: any): Date => {
  throw new Error('NotImplemented');
};
