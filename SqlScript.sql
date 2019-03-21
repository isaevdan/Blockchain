SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OutTransactions](
	[TxId] [nvarchar](120) NOT NULL,
	[Amount] [decimal](18, 9) NOT NULL,
	[Address] [nvarchar](120) NOT NULL,
	[TimeReceived] [datetime] NOT NULL,
	[Confirmations] [int] NOT NULL,
	[FromWallet] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OutTransactions] PRIMARY KEY CLUSTERED 
(
	[TxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[InTransactions](
	[TxId] [nvarchar](120) NOT NULL,
	[Amount] [decimal](18, 9) NOT NULL,
	[Address] [nvarchar](120) NOT NULL,
	[TimeReceived] [datetime] NOT NULL,
	[Confirmations] [int] NOT NULL,
	[IsNew] [bit] NOT NULL,
 CONSTRAINT [PK_InTransactions] PRIMARY KEY CLUSTERED 
(
	[TxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InTransactions] ADD  CONSTRAINT [DF_InTransactions_IsNew]  DEFAULT ((1)) FOR [IsNew]
GO

CREATE TABLE [dbo].[Wallets](
	[Name] [nvarchar](50) NOT NULL,
	[Balance] [decimal](18, 9) NOT NULL,
 CONSTRAINT [PK_Wallets] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE PROCEDURE [dbo].[InTransactions_GetLast]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT [TxId]
      ,[Amount]
      ,[Address]
      ,[TimeReceived]
      ,[Confirmations]
  FROM [InTransactions]
  WHERE IsNew = 1 OR Confirmations < 3

  update [InTransactions] set IsNew = 0
END

GO


CREATE PROCEDURE [dbo].[InTransactions_Store]
	-- Add the parameters for the stored procedure here
	@TxId nvarchar(120),
	@Amount [decimal](18, 9),
	@Address [nvarchar](120),
	@TimeReceived DATETIME,
	@Confirmations [int]
AS
BEGIN
	if exists (select * from InTransactions where TxId = @TxId)
	begin
	   update InTransactions set Confirmations = @Confirmations where TxId = @TxId
	end
	else
	begin
	   insert into InTransactions (TxId, Amount, Address, TimeReceived, Confirmations)
	   values (@TxId, @Amount, @Address, @TimeReceived, @Confirmations)
	end
END

GO


CREATE PROCEDURE [dbo].[OutTransactions_Store]
	-- Add the parameters for the stored procedure here
	@TxId nvarchar(120),
	@Amount [decimal](18, 9),
	@Address [nvarchar](120),
	@TimeReceived DATETIME,
	@FromWallet nvarchar(50),
	@Confirmations [int]
AS
BEGIN
	if exists (select * from OutTransactions where TxId = @TxId)
	begin
	   update OutTransactions set Confirmations = @Confirmations where TxId = @TxId
	end
	else
	begin
	   insert into OutTransactions (TxId, Amount, Address, TimeReceived, Confirmations, FromWallet)
	   values (@TxId, @Amount, @Address, @TimeReceived, @Confirmations, @FromWallet)
	end
END

GO


CREATE PROCEDURE [dbo].[Wallet_Store]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(50),
	@Balance [decimal](18, 9)
AS
BEGIN
	if exists (select * from Wallets where Name = @Name)
	begin
	   update Wallets set Balance = @Balance where Name = @Name
	end
	else
	begin
	   insert into Wallets (Name, Balance)
	   values (@Name, @Balance)
	end
END

GO


CREATE PROCEDURE [dbo].[Wallets_GetAll]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [Name]
           ,[Balance]
	FROM Wallets
END

GO


CREATE PROCEDURE [dbo].[InTransactions_GetNeedToUpdate]
AS
BEGIN

  SET NOCOUNT ON;

  SELECT [TxId]
  FROM [InTransactions]
  where Confirmations <= 6

END

GO

CREATE PROCEDURE [dbo].[OutTransactions_GetLastTransactionDate]
AS
BEGIN

  SET NOCOUNT ON;

  SELECT top 1 TimeReceived
  FROM OutTransactions
  order by TimeReceived desc

END


Create PROCEDURE [dbo].[Wallets_ForTransaction]
	@Amount [decimal](18, 9)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [Name]
           ,[Balance]
	FROM Wallets
	where Balance > @Amount
END

GO


