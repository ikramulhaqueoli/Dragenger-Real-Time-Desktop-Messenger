USE [master]
GO
/****** Object:  Database [DragengerServerDB]    Script Date: 23-06-2020 01:49:57 ******/
CREATE DATABASE [DragengerServerDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DragengerServerDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\DragengerServerDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DragengerServerDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\DragengerServerDB_log.ldf' , SIZE = 1280KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [DragengerServerDB] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DragengerServerDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DragengerServerDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DragengerServerDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DragengerServerDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DragengerServerDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DragengerServerDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [DragengerServerDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [DragengerServerDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DragengerServerDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DragengerServerDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DragengerServerDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DragengerServerDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DragengerServerDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DragengerServerDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DragengerServerDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DragengerServerDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DragengerServerDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DragengerServerDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DragengerServerDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DragengerServerDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DragengerServerDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DragengerServerDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DragengerServerDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DragengerServerDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DragengerServerDB] SET  MULTI_USER 
GO
ALTER DATABASE [DragengerServerDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DragengerServerDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DragengerServerDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DragengerServerDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DragengerServerDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DragengerServerDB] SET QUERY_STORE = OFF
GO
USE [DragengerServerDB]
GO
/****** Object:  Table [dbo].[Block_List_Map]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Block_List_Map](
	[Blocker_ID] [bigint] NOT NULL,
	[Blocked_ID] [bigint] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Consumers]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consumers](
	[User_ID] [bigint] NOT NULL,
	[Name] [nvarchar](35) NOT NULL,
	[Profile_img_ID] [nvarchar](50) NULL,
	[Birthdate] [date] NULL,
	[Phone] [nvarchar](20) NULL,
	[Gender] [smallint] NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Consumers] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversations](
	[Id] [bigint] IDENTITY(100,1) NOT NULL,
	[Type] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Devices_Bind_Map]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Devices_Bind_Map](
	[User_id] [bigint] NOT NULL,
	[Device_Mac] [nchar](12) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Duet_Conversations]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Duet_Conversations](
	[Conversation_Id] [bigint] NOT NULL,
	[Member_Id_1] [bigint] NOT NULL,
	[Member_Id_2] [bigint] NOT NULL,
 CONSTRAINT [PK_Duet_Conversations] PRIMARY KEY CLUSTERED 
(
	[Conversation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Friend_request_Map]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friend_request_Map](
	[Sender_Id] [bigint] NOT NULL,
	[Receiver_Id] [bigint] NOT NULL,
	[Time] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Friendship_Map]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friendship_Map](
	[User_id_1] [bigint] NOT NULL,
	[User_id_2] [bigint] NOT NULL,
	[Added_on] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Group_conversations]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group_conversations](
	[Conversation_Id] [bigint] NOT NULL,
	[Group_name] [nvarchar](35) NOT NULL,
	[Icon_ID] [nvarchar](20) NULL,
 CONSTRAINT [PK_Group_conversations] PRIMARY KEY CLUSTERED 
(
	[Conversation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Group_Member_Map]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group_Member_Map](
	[Conversation_Id] [bigint] NOT NULL,
	[Member_Id] [bigint] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nuntii]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nuntii](
	[Id] [bigint] IDENTITY(100,1) NOT NULL,
	[Sender_id] [bigint] NOT NULL,
	[Conversation_id] [bigint] NOT NULL,
	[Sent_time] [datetime] NOT NULL,
	[Delivery_time] [datetime] NULL,
	[Seen_time] [datetime] NULL,
	[Text] [ntext] NULL,
	[Content_ID] [ntext] NULL,
 CONSTRAINT [PK_Nuntii] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nuntius_Owner_map]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nuntius_Owner_map](
	[Nuntias_Id] [bigint] NOT NULL,
	[Owner_Id] [bigint] NOT NULL,
	[Delivery_Status] [smallint] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](20) NOT NULL,
	[User_type] [nvarchar](15) NOT NULL,
	[Password] [nvarchar](50) NULL,
	[Last_Active] [nvarchar](50) NULL,
	[Verified] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Verification_Codes]    Script Date: 23-06-2020 01:49:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Verification_Codes](
	[User_Id] [bigint] NOT NULL,
	[Purpose] [nvarchar](15) NOT NULL,
	[Verification_Code] [nchar](6) NOT NULL,
	[Assigned_Time] [datetime] NOT NULL,
	[Times_Checked] [smallint] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Consumers] ADD  CONSTRAINT [DF_Consumers_Gender]  DEFAULT ((0)) FOR [Gender]
GO
ALTER TABLE [dbo].[Friend_request_Map] ADD  CONSTRAINT [DF_Friend_request_Map_Time]  DEFAULT (getdate()) FOR [Time]
GO
ALTER TABLE [dbo].[Friendship_Map] ADD  CONSTRAINT [DF_Friendship_Map_Added_on]  DEFAULT (getdate()) FOR [Added_on]
GO
ALTER TABLE [dbo].[Nuntius_Owner_map] ADD  CONSTRAINT [DF_Nuntius_Owner_map_Delivery_Status]  DEFAULT ((0)) FOR [Delivery_Status]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Verified]  DEFAULT ((0)) FOR [Verified]
GO
ALTER TABLE [dbo].[Verification_Codes] ADD  CONSTRAINT [DF_Verifications_Codes_Assigned_Time]  DEFAULT (sysdatetime()) FOR [Assigned_Time]
GO
ALTER TABLE [dbo].[Verification_Codes] ADD  CONSTRAINT [DF_Verifications_Codes_Times_Checked]  DEFAULT ((0)) FOR [Times_Checked]
GO
ALTER TABLE [dbo].[Block_List_Map]  WITH CHECK ADD  CONSTRAINT [FK_Block_List_Map_Consumers_1] FOREIGN KEY([Blocker_ID])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Block_List_Map] CHECK CONSTRAINT [FK_Block_List_Map_Consumers_1]
GO
ALTER TABLE [dbo].[Block_List_Map]  WITH CHECK ADD  CONSTRAINT [FK_Block_List_Map_Consumers_2] FOREIGN KEY([Blocked_ID])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Block_List_Map] CHECK CONSTRAINT [FK_Block_List_Map_Consumers_2]
GO
ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Users] FOREIGN KEY([User_ID])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Users]
GO
ALTER TABLE [dbo].[Devices_Bind_Map]  WITH CHECK ADD  CONSTRAINT [FK_Devices_Bind_Map_User] FOREIGN KEY([User_id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Devices_Bind_Map] CHECK CONSTRAINT [FK_Devices_Bind_Map_User]
GO
ALTER TABLE [dbo].[Duet_Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Duet_Conversations_Consumers_1] FOREIGN KEY([Member_Id_1])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Duet_Conversations] CHECK CONSTRAINT [FK_Duet_Conversations_Consumers_1]
GO
ALTER TABLE [dbo].[Duet_Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Duet_Conversations_Consumers_2] FOREIGN KEY([Member_Id_2])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Duet_Conversations] CHECK CONSTRAINT [FK_Duet_Conversations_Consumers_2]
GO
ALTER TABLE [dbo].[Duet_Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Duet_Conversations_Conversations] FOREIGN KEY([Conversation_Id])
REFERENCES [dbo].[Conversations] ([Id])
GO
ALTER TABLE [dbo].[Duet_Conversations] CHECK CONSTRAINT [FK_Duet_Conversations_Conversations]
GO
ALTER TABLE [dbo].[Friend_request_Map]  WITH CHECK ADD  CONSTRAINT [FK_Friend_request_Map_Consumers_r] FOREIGN KEY([Receiver_Id])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Friend_request_Map] CHECK CONSTRAINT [FK_Friend_request_Map_Consumers_r]
GO
ALTER TABLE [dbo].[Friend_request_Map]  WITH CHECK ADD  CONSTRAINT [FK_Friend_request_Map_Consumers_s] FOREIGN KEY([Sender_Id])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Friend_request_Map] CHECK CONSTRAINT [FK_Friend_request_Map_Consumers_s]
GO
ALTER TABLE [dbo].[Friendship_Map]  WITH CHECK ADD  CONSTRAINT [FK_Friends_Map_Consumers_1] FOREIGN KEY([User_id_1])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Friendship_Map] CHECK CONSTRAINT [FK_Friends_Map_Consumers_1]
GO
ALTER TABLE [dbo].[Friendship_Map]  WITH CHECK ADD  CONSTRAINT [FK_Friends_Map_Consumers_2] FOREIGN KEY([User_id_2])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Friendship_Map] CHECK CONSTRAINT [FK_Friends_Map_Consumers_2]
GO
ALTER TABLE [dbo].[Group_conversations]  WITH CHECK ADD  CONSTRAINT [FK_Group_conversations_Conversations] FOREIGN KEY([Conversation_Id])
REFERENCES [dbo].[Conversations] ([Id])
GO
ALTER TABLE [dbo].[Group_conversations] CHECK CONSTRAINT [FK_Group_conversations_Conversations]
GO
ALTER TABLE [dbo].[Group_Member_Map]  WITH CHECK ADD  CONSTRAINT [FK_Group_Member_Map_Consumers] FOREIGN KEY([Member_Id])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Group_Member_Map] CHECK CONSTRAINT [FK_Group_Member_Map_Consumers]
GO
ALTER TABLE [dbo].[Group_Member_Map]  WITH CHECK ADD  CONSTRAINT [FK_Group_Member_Map_Group_conversations] FOREIGN KEY([Conversation_Id])
REFERENCES [dbo].[Group_conversations] ([Conversation_Id])
GO
ALTER TABLE [dbo].[Group_Member_Map] CHECK CONSTRAINT [FK_Group_Member_Map_Group_conversations]
GO
ALTER TABLE [dbo].[Nuntii]  WITH CHECK ADD  CONSTRAINT [FK_Nuntii_Consumers] FOREIGN KEY([Sender_id])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Nuntii] CHECK CONSTRAINT [FK_Nuntii_Consumers]
GO
ALTER TABLE [dbo].[Nuntii]  WITH CHECK ADD  CONSTRAINT [FK_Nuntii_Conversations] FOREIGN KEY([Conversation_id])
REFERENCES [dbo].[Conversations] ([Id])
GO
ALTER TABLE [dbo].[Nuntii] CHECK CONSTRAINT [FK_Nuntii_Conversations]
GO
ALTER TABLE [dbo].[Nuntius_Owner_map]  WITH CHECK ADD  CONSTRAINT [FK_Nuntius_Owner_map_Consumers] FOREIGN KEY([Owner_Id])
REFERENCES [dbo].[Consumers] ([User_ID])
GO
ALTER TABLE [dbo].[Nuntius_Owner_map] CHECK CONSTRAINT [FK_Nuntius_Owner_map_Consumers]
GO
ALTER TABLE [dbo].[Nuntius_Owner_map]  WITH CHECK ADD  CONSTRAINT [FK_Nuntius_Owner_map_Nuntii] FOREIGN KEY([Nuntias_Id])
REFERENCES [dbo].[Nuntii] ([Id])
GO
ALTER TABLE [dbo].[Nuntius_Owner_map] CHECK CONSTRAINT [FK_Nuntius_Owner_map_Nuntii]
GO
ALTER TABLE [dbo].[Verification_Codes]  WITH CHECK ADD  CONSTRAINT [FK_Email_Verifications_Users] FOREIGN KEY([User_Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Verification_Codes] CHECK CONSTRAINT [FK_Email_Verifications_Users]
GO
USE [master]
GO
ALTER DATABASE [DragengerServerDB] SET  READ_WRITE 
GO
