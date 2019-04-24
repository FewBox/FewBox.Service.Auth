USE 'authentication';
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for api
-- ----------------------------
DROP TABLE IF EXISTS `api`;
CREATE TABLE `api` (
  `Id` char(36) NOT NULL,
  `Controller` varchar(45) DEFAULT NULL,
  `Action` varchar(45) DEFAULT NULL,
  `SecurityObjectId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of api
-- ----------------------------
INSERT INTO `api` VALUES ('052ed243-69d5-4fe8-b8f1-efb5f329aa69', 'Roles', 'GetTotalNumber', 'f1563584-777c-46da-a3e6-8a7b0a2b6008', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-09-05 03:14:32', '2018-09-05 03:14:32');
INSERT INTO `api` VALUES ('059d04ee-02be-4599-8b55-2dbd8dadf1b0', 'Roles', 'Delete', '257900e7-fb1a-4b54-9d0a-ca1e21b80071', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:22:49', '2018-07-30 08:07:32');
INSERT INTO `api` VALUES ('07d1ead9-f760-48f6-a850-838260338c0e', 'Group_Users', 'Get', '8e263330-09f6-4b9a-a673-8f80b5d57ffb', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:17:11', '2018-07-30 08:01:30');
INSERT INTO `api` VALUES ('0a529896-b691-458d-b5b9-bffbe5f7aced', 'Users', 'Get', '0e049ff4-4044-43d6-92ec-61bbd0cc6d63', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:23:33', '2018-07-30 08:01:39');
INSERT INTO `api` VALUES ('176880cb-d66a-4f78-a726-b2e7c6b950e8', 'Authentication', 'SentValidateCode', '0fab1c92-adc4-4346-99cb-d5a2874365ec', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:20:42', '2018-07-30 08:04:40');
INSERT INTO `api` VALUES ('17b402e3-cb4e-4830-9b7c-2571f15f79ef', 'Users', 'Put', 'e5b46e47-f3c1-4cd2-9ebb-9349ace64b2b', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:23:48', '2018-07-30 08:05:42');
INSERT INTO `api` VALUES ('1b7ae448-2a24-4ea7-a6a4-a8ca79fe1bbb', 'Principal_Roles', 'Delete', 'e03f8686-53f3-43ea-a78b-9a07a81632d3', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:20:49', '2018-07-29 12:20:49');
INSERT INTO `api` VALUES ('213b9d02-6b52-4d4f-8a2f-665179f52ba9', 'Role_SecurityObjects', 'GrantPermission', '7b16bbfa-3f30-445c-babc-f88ce9c25735', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:22:00', '2018-07-30 08:04:56');
INSERT INTO `api` VALUES ('243294b5-d8a9-4ffd-9f8d-23de821505d4', 'Modules', 'Post', 'bf5bb792-50a3-4963-b998-96e80a68873e', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:19:05', '2018-07-30 08:06:02');
INSERT INTO `api` VALUES ('254ea36b-f1f9-409f-9dc1-c76c832fac84', 'Modules', 'GetTotalNumber', '7a385923-7138-4bf7-8de5-1e5b9e0ff5dc', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-09-05 03:14:23', '2018-09-05 03:14:23');
INSERT INTO `api` VALUES ('263dc570-7aa2-45d6-9016-2064f61b24a1', 'Group_Users', 'Post', '4a5f92d4-a01d-4a4d-8c00-fea7b92caee0', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:17:20', '2018-07-30 08:06:11');
INSERT INTO `api` VALUES ('33f7182f-fac7-4dfe-b609-1b7fc1ed8078', 'Group_Users', 'Delete', 'dd5ffc28-94f5-40ab-969b-5f964d3fcd89', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:17:35', '2018-07-30 08:06:35');
INSERT INTO `api` VALUES ('389886ba-1464-4bab-88a0-844271d89074', 'Apis', 'Get', 'ad4a3540-7a1a-44e8-b0f0-de901ab20889', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-28 02:29:12', '2018-07-29 12:11:08');
INSERT INTO `api` VALUES ('3c8527a2-7998-4415-92d1-295203ff4d5e', 'Users', 'GetByKeyword', 'a05e85cd-9b6c-4e40-ac03-2cefaed3399f', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-10-26 13:11:43', '2018-10-26 13:11:43');
INSERT INTO `api` VALUES ('453a582d-6113-403e-bc1c-c9446119a123', 'Groups', 'Delete', '1b052d57-a9f9-4aac-ab05-44ff8b3fe1fe', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:18:26', '2018-07-29 12:18:26');
INSERT INTO `api` VALUES ('4cfc7f46-8e7c-434d-beca-98541fb50dda', 'Principal_Roles', 'Post', '00283bb1-2e93-4e33-9b53-02259bb2d918', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:20:37', '2018-07-29 12:20:37');
INSERT INTO `api` VALUES ('4f93987d-5dff-4754-8db7-c33dd42eef88', 'Modules', 'GetByRoot', '6a88475f-dadc-4929-a97f-2296d518b0f7', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '2018-10-26 07:12:58', '2018-10-26 07:12:58');
INSERT INTO `api` VALUES ('692cc216-2ced-45e7-9b6a-766733ceacde', 'Group_Users', 'Put', '0ef1ef35-8aa7-438f-ba5c-1d897d2389ea', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:17:27', '2018-07-29 12:17:27');
INSERT INTO `api` VALUES ('69bada06-1a29-4a29-90fa-98abff8073c4', 'Role_SecurityObjects', 'RemovePermission', 'b10ef11c-9928-4882-a563-5f99dde4449b', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:22:07', '2018-07-29 12:22:07');
INSERT INTO `api` VALUES ('6c8e4b6a-4593-45d5-9cbb-a1a45703ed06', 'Users', 'GetTotalNumber', 'deb9fb2b-3fc6-4542-a5c9-ca314b683d93', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-09-05 03:14:15', '2018-09-05 03:14:15');
INSERT INTO `api` VALUES ('81b9b538-9d08-45ab-8669-3bf01f7b4186', 'Groups', 'GetTotalNumber', '6b506fbe-7080-4ba7-a308-17261c718085', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-09-05 03:14:03', '2018-09-05 03:14:03');
INSERT INTO `api` VALUES ('82c88703-b8c9-4418-a9b6-6cf391820ba8', 'Users', 'GetByIds', '6bbcc544-3011-4b51-8e53-f7e4a763a7ae', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-10-31 03:06:48', '2018-10-31 03:06:48');
INSERT INTO `api` VALUES ('83383ad8-db25-4d39-a037-22c3fe4cf03f', 'Authentication', 'ChangePassword', 'f32b889b-296d-42a0-be5e-62d673a56656', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:16:22', '2018-07-29 12:16:22');
INSERT INTO `api` VALUES ('834f35f1-836e-4718-aeb2-82048691246c', 'Modules', 'Get', '9ed45169-e9fb-4a09-98a9-b83d0cc502b3', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:18:58', '2018-07-30 08:10:07');
INSERT INTO `api` VALUES ('864999b6-6cdf-4d5d-a572-572fd06bb442', 'Apis', 'Delete', '74394d87-d1e6-43ac-983c-791b538692cc', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:12:59', '2018-07-30 08:09:58');
INSERT INTO `api` VALUES ('933d0634-ed4f-4b3b-8555-8acb01d003f8', 'Users', 'Post', '2b90effd-3457-400b-ac3f-a83f993b0641', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:23:42', '2018-07-30 08:52:18');
INSERT INTO `api` VALUES ('9e660bc3-83ae-4b74-b8a8-1280bede83e5', 'Registry', 'PostAsync', '9f30159b-a34e-4a3b-8304-b737119dab8f', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:21:17', '2018-07-30 08:52:44');
INSERT INTO `api` VALUES ('a462b5c6-daf8-4830-b1af-f390b960c6e5', 'Roles', 'GetByModuleId', '97f69ff5-cf6a-4922-ad82-65cdb87ba2d6', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '2018-10-26 07:13:15', '2018-10-26 07:13:15');
INSERT INTO `api` VALUES ('a7781b5d-7eb7-43f3-b443-077f10b9aabf', 'Roles', 'GetByPermissionId', 'fb7760a9-a0ba-4739-8811-c793c78eb8fe', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-08-03 06:12:54', '2018-08-03 06:21:06');
INSERT INTO `api` VALUES ('af71e1a4-05e5-47e2-9e9a-ff2cfb3bc250', 'Users', 'GetTotalNumberByRoleCode', 'cee1a2ee-6b2f-43d3-bca5-3cc9d98103d5', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-09-05 03:14:46', '2018-09-05 03:14:46');
INSERT INTO `api` VALUES ('b15156f7-4fe6-45f9-9248-62e8d89b0ce2', 'Roles', 'GetByApiId', '73ce55b9-d2a9-4e9d-b6bb-57dc971922d0', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '2018-10-26 07:12:32', '2018-10-26 07:12:32');
INSERT INTO `api` VALUES ('b2678666-c10e-4637-af91-125e8ea363b3', 'Principal_Roles', 'Get', '8a20c501-1dde-4a66-a943-6be1791cde07', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:19:21', '2018-07-30 08:52:34');
INSERT INTO `api` VALUES ('b27d27e5-f17f-4ad9-825f-fc3590ff1ca3', 'Groups', 'Put', '307eebf0-66f0-4d31-8926-fe95f31ad89c', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:18:19', '2018-07-30 08:52:51');
INSERT INTO `api` VALUES ('b9ad82e0-9f98-4d33-a196-b4cddc8094c0', 'Authentication', 'ValidatePassword', '836365dc-3acb-4078-b1d6-f4df7292bd48', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:16:37', '2018-07-30 08:53:34');
INSERT INTO `api` VALUES ('bbf0473c-418e-43be-984d-368fcb6b1d00', 'Roles', 'Put', '55b2b95b-47b0-44d0-be7e-d647e6aaeda2', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:22:40', '2018-07-30 08:53:14');
INSERT INTO `api` VALUES ('c743ff6c-994d-49f7-976a-b3d9a9cf4a03', 'Roles', 'Get', '54264361-5f7f-4c11-aac2-0bccd07b9a74', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:22:26', '2018-07-30 08:53:07');
INSERT INTO `api` VALUES ('c7cef899-c929-4afc-96ee-7d2b5b82633c', 'Apis', 'GetByKeyword', '71964588-1f20-4da1-8c8b-b73cf96fd792', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-07-31 08:30:56', '2018-07-31 08:58:39');
INSERT INTO `api` VALUES ('ca929355-927e-45e7-8174-7db624d4d73d', 'Roles', 'Post', 'd80af451-819d-4966-96f2-00256c42ae84', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:22:33', '2018-07-30 08:53:00');
INSERT INTO `api` VALUES ('cdedebe1-1ec6-49a1-a9d8-320717fb4a2b', 'Roles', 'GetByUserId', '143ded6e-31af-4e46-a29f-43686a8a9551', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-07-31 10:52:21', '2018-07-31 10:52:21');
INSERT INTO `api` VALUES ('d489a799-7ded-427c-8da2-cd1c9cfc3a09', 'Role_SecurityObjects', 'Get', '269b3310-27c9-47e2-9fa0-32af73cb8fd3', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:21:47', '2018-07-30 08:54:36');
INSERT INTO `api` VALUES ('d945e061-7783-47af-a982-d6ced2482c21', 'Apis', 'Post', '43d71626-4035-4104-93b9-53dc9fb74098', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:13:23', '2018-07-30 08:54:31');
INSERT INTO `api` VALUES ('de06eeae-910a-42f4-8e00-3d65b3f03ecf', 'Groups', 'Get', 'cf0d18e7-e82c-4d07-b68e-4eb40999b120', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:17:59', '2018-07-30 08:54:21');
INSERT INTO `api` VALUES ('e9048a6c-134b-4da9-8ba3-a071b52e3180', 'Users', 'ResetPassword', '95d94861-50f8-484e-94a6-ef1d9f57ad4a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-07-31 11:25:42', '2018-07-31 11:25:42');
INSERT INTO `api` VALUES ('eae58995-4fce-49d4-87bc-7c147593ab06', 'Users', 'Delete', 'c3585de8-dec7-49fd-810a-0610792c892f', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:23:54', '2018-07-30 08:54:15');
INSERT INTO `api` VALUES ('ec3256f2-c164-42be-8671-aadb6694a44a', 'Modules', 'Delete', 'daf7eb20-a506-4ce7-9bf8-b8be8bb32325', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-10-26 13:20:33', '2018-10-26 13:20:33');
INSERT INTO `api` VALUES ('f334b36f-e195-43d5-a17c-5b5d5152846b', 'Modules', 'Put', '8549c896-09ae-4fc1-996b-7a8bd0b1cd96', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:19:12', '2018-07-30 08:54:06');
INSERT INTO `api` VALUES ('fd10fc2a-fc00-4067-a2d1-847da93d48eb', 'Groups', 'Post', 'fd9ee110-a8a6-43da-8de2-e23e3335f09b', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:18:09', '2018-07-30 08:53:53');
INSERT INTO `api` VALUES ('fe8a771c-3d98-4f22-b121-2a3e174360e1', 'Roles', 'GetBySecurityObjectId', 'fe8a771c-3d98-4f22-b121-2a3e174360e2', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-28 02:29:12', '2018-07-28 02:29:12');
INSERT INTO `api` VALUES ('fe8a771c-3d98-4f22-b121-2a3e174360ef', 'Apis', 'Put', 'ad4a3540-7a1a-44e8-b0f0-de901ab20889', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-28 02:29:12', '2018-07-29 12:12:37');

-- ----------------------------
-- Table structure for api_recycle
-- ----------------------------
DROP TABLE IF EXISTS `api_recycle`;
CREATE TABLE `api_recycle` (
  `Id` char(36) NOT NULL,
  `Controller` varchar(45) DEFAULT NULL,
  `Action` varchar(45) DEFAULT NULL,
  `SecurityObjectId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of api_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for group
-- ----------------------------
DROP TABLE IF EXISTS `group`;
CREATE TABLE `group` (
  `Id` char(36) NOT NULL,
  `ParentId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `PrincipalId` char(36) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of group
-- ----------------------------

-- ----------------------------
-- Table structure for group_recycle
-- ----------------------------
DROP TABLE IF EXISTS `group_recycle`;
CREATE TABLE `group_recycle` (
  `Id` char(36) NOT NULL,
  `ParentId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `PrincipalId` char(36) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of group_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for group_user
-- ----------------------------
DROP TABLE IF EXISTS `group_user`;
CREATE TABLE `group_user` (
  `Id` char(36) NOT NULL,
  `GroupId` char(36) DEFAULT NULL,
  `UserId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of group_user
-- ----------------------------

-- ----------------------------
-- Table structure for group_user_recycle
-- ----------------------------
DROP TABLE IF EXISTS `group_user_recycle`;
CREATE TABLE `group_user_recycle` (
  `Id` char(36) NOT NULL,
  `GroupId` char(36) DEFAULT NULL,
  `UserId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of group_user_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for module
-- ----------------------------
DROP TABLE IF EXISTS `module`;
CREATE TABLE `module` (
  `Id` char(36) NOT NULL,
  `Key` varchar(45) DEFAULT NULL,
  `ParentId` char(36) DEFAULT NULL,
  `SecurityObjectId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of module
-- ----------------------------

-- ----------------------------
-- Table structure for module_recycle
-- ----------------------------
DROP TABLE IF EXISTS `module_recycle`;
CREATE TABLE `module_recycle` (
  `Id` char(36) NOT NULL,
  `Key` varchar(45) DEFAULT NULL,
  `ParentId` char(36) DEFAULT NULL,
  `SecurityObjectId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for principal
-- ----------------------------
DROP TABLE IF EXISTS `principal`;
CREATE TABLE `principal` (
  `Id` char(36) NOT NULL,
  `PrincipalType` int(11) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of principal
-- ----------------------------
INSERT INTO `principal` VALUES ('026c56e3-17f0-4619-b5e9-7c9f17066586', '0', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-09-13 07:14:30', '2018-09-13 07:14:30', 'fewbox', null);
-- ----------------------------
-- Table structure for principal_recycle
-- ----------------------------
DROP TABLE IF EXISTS `principal_recycle`;
CREATE TABLE `principal_recycle` (
  `Id` char(36) NOT NULL,
  `PrincipalType` int(11) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of principal_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for principal_role
-- ----------------------------
DROP TABLE IF EXISTS `principal_role`;
CREATE TABLE `principal_role` (
  `Id` char(36) NOT NULL,
  `PrincipalId` char(36) DEFAULT NULL,
  `RoleId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of principal_role
-- ----------------------------
INSERT INTO `principal_role` VALUES ('009f96c6-0031-452c-8795-b0072eddf745', 'e14463f3-f637-4f88-9ff8-2ffaa46233b8', 'd77e4183-563e-470c-8331-c9a9fc22f78a', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-08-21 09:39:32', '2018-08-21 09:39:32');

-- ----------------------------
-- Table structure for principal_role_recycle
-- ----------------------------
DROP TABLE IF EXISTS `principal_role_recycle`;
CREATE TABLE `principal_role_recycle` (
  `Id` char(36) NOT NULL,
  `PrincipalId` char(36) DEFAULT NULL,
  `RoleId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of principal_role_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS `role`;
CREATE TABLE `role` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Code` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of role
-- ----------------------------
INSERT INTO `role` VALUES ('62652f41-94b9-4039-81d4-2456d74e6400', 'App Admin', 'R_ADMIN', null, '00000000-0000-0000-0000-000000000000', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-07-28 02:29:12', '2018-08-02 07:58:44');
INSERT INTO `role` VALUES ('a08f61b5-5f1f-41d6-bd8f-7fd84d459238', 'Anonymous 匿名', 'NULL', null, '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '13f4999d-aab7-4782-b91b-0c2d230d5dfb', '2018-10-19 09:32:35', '2018-10-19 09:32:59');


-- ----------------------------
-- Table structure for role_recycle
-- ----------------------------
DROP TABLE IF EXISTS `role_recycle`;
CREATE TABLE `role_recycle` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Code` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of role_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for role_security
-- ----------------------------
DROP TABLE IF EXISTS `role_security`;
CREATE TABLE `role_security` (
  `Id` char(36) NOT NULL,
  `SecurityObjectId` char(36) DEFAULT NULL,
  `RoleId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of role_security
-- ----------------------------
INSERT INTO `role_security` VALUES ('000ad63b-b694-4eee-b508-4f9a8d3660b2', '6bbcc544-3011-4b51-8e53-f7e4a763a7ae', '4ce65237-eddc-450a-9239-f0347faf20df', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', 'cc584046-ba27-4fce-8b2b-6bc69c24676a', '2018-10-31 03:06:48', '2018-10-31 03:06:48');

-- ----------------------------
-- Table structure for role_security_recycle
-- ----------------------------
DROP TABLE IF EXISTS `role_security_recycle`;
CREATE TABLE `role_security_recycle` (
  `Id` char(36) NOT NULL,
  `SecurityObjectId` char(36) DEFAULT NULL,
  `RoleId` char(36) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of role_security_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for securityobject
-- ----------------------------
DROP TABLE IF EXISTS `securityobject`;
CREATE TABLE `securityobject` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of securityobject
-- ----------------------------
INSERT INTO `securityobject` VALUES ('00283bb1-2e93-4e33-9b53-02259bb2d918', null, null, '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-07-29 12:20:37', '2018-07-29 12:20:37');
-- ----------------------------
-- Table structure for securityobject_recycle
-- ----------------------------
DROP TABLE IF EXISTS `securityobject_recycle`;
CREATE TABLE `securityobject_recycle` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of securityobject_recycle
-- ----------------------------

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `Id` char(36) NOT NULL,
  `Type` int(11) DEFAULT NULL,
  `Salt` char(36) DEFAULT NULL,
  `SaltMD5Password` varchar(45) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,
  `Mobile` varchar(45) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `PrincipalId` char(36) DEFAULT NULL,
  `DisplayName` varchar(45) DEFAULT NULL,
  `Department` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES ('00538cf2-227b-4460-a572-a282928edbca', '0', '466fe1e1-1efc-4ca9-9409-ee0dc624bf8e', 'iBXkei3lTm/3jx7qYlDv/g==', null, null, '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '2018-08-16 09:02:27', '2018-08-16 09:02:27', '026c56e3-17f0-4619-b5e9-7c9f17066586', null, null);

-- ----------------------------
-- Table structure for user_recycle
-- ----------------------------
DROP TABLE IF EXISTS `user_recycle`;
CREATE TABLE `user_recycle` (
  `Id` char(36) NOT NULL,
  `Salt` char(36) DEFAULT NULL,
  `SaltMD5Password` varchar(45) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,
  `Mobile` varchar(45) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  `PrincipalId` char(36) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of user_recycle
-- ----------------------------
