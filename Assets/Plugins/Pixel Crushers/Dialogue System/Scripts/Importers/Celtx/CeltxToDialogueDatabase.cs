#if USE_CELTX
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.Celtx
{
    /// <summary>
    /// This class does the actual work of converting Celtx Data (Raw) into a dialogue database.
    /// </summary>
    public class CeltxToDialogueDatabase
    {
        Template template = Template.FromDefault();
        DialogueDatabase database;
        CeltxData celtxData;

        public DialogueDatabase ProcessRawCeltxData(CeltxDataRaw celtxDataRaw, DialogueDatabase database, bool importGameplayScriptText, bool importBreakdownCatalogContent, bool checkSequenceSyntax)
        {
            try
            {
                this.database = database;
                celtxData = new CeltxData();

                List<CxContent> docContentList = celtxDataRaw.doc.content;

                Debug.Log("Converting Celtx Data to Dialogue System database");

                CxContent cxMetadata = docContentList.Find(c => c.type.Equals("cxmetadata"));
                ProcessCxCatalog(cxMetadata);
                ProcessCxVariables(cxMetadata);
                ProcessCxConditions(cxMetadata);
                List<CxContent> sequenceList = docContentList.Where(c => c.type.Equals("cxsequence")).ToList();

                ProcessSequenceListFirstPass(sequenceList);

                //sequenceList.ForEach(c => ProcessCeltxSequenceNode(c));

                //ResolvePendingLinks();

                if (checkSequenceSyntax) CheckSequenceSyntax();
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e);
            }

            return database;
        }

        private void ProcessCxCatalog(CxContent cxMetadata)
        {
            List<CxContent> cxCatalogContent;
            try
            {
                cxCatalogContent = cxMetadata.content.Find(c => c.type.Equals("cxcatalog")).content;
            }
            catch (System.Exception)
            {
                Debug.Log("Skipping catalog because it is null or empty");
                return;
            }

            foreach (CxContent contentObject in cxCatalogContent)
            {
                switch (contentObject.attrs.type)
                {
                    case "character":
                        ConvertCharacterCatalogEntryToDSActor(contentObject.attrs);
                        break;

                    case "location":
                        ConvertLocationCatalogEntryToDSLocation(contentObject.attrs);
                        break;

                    case "item":
                        ConvertItemCatalogEntryToDSItem(contentObject.attrs);
                        break;

                    case "branch":
                    case "jump":
                    case "sequence":
                        ExtractCustomVarsList(contentObject.attrs);
                        break;
                }
            }
        }

        private void ProcessCxVariables(CxContent cxMetadata)
        {
            List<CxVariable> cxVariablesList;
            try
            {
                cxVariablesList = cxMetadata.content.Find(c => c.type.Equals("cxconditions")).attrs.variables;
            }
            catch (System.Exception)
            {
                Debug.Log("Skipping variables because there are none");
                return;
            }

            foreach (CxVariable variableObject in cxVariablesList)
            {
                ConvertCeltxVariableToDSVariable(variableObject);
            }
        }

        private void ProcessCxConditions(CxContent cxMetadata)
        {
            List<CxContent> cxConditionsContent;
            try
            {
                cxConditionsContent = cxMetadata.content.Find(c => c.type.Equals("cxconditions")).content;
            }
            catch (System.Exception)
            {
                Debug.Log("Skipping variables because there are none");
                return;
            }

            
            foreach (CxContent contentObject in cxConditionsContent)
            {
                ExtractCeltxCondition(contentObject.attrs);
            }

            foreach (CeltxCondition condition in celtxData.conditions)
            {
                GenerateLuaCondition(condition);
            }
        }

        private void AppendToField(List<Field> fields, string title, string value, FieldType fieldType)
        {
            string currentFieldValue = Field.LookupValue(fields, title);
            if (value == null || value.Equals("") || currentFieldValue != null && currentFieldValue.Equals(value)) { return; }

            string updatedString;
            if (currentFieldValue == null || currentFieldValue.Equals("") || currentFieldValue.Equals("[]")) { updatedString = value; }
            else { updatedString = currentFieldValue + " " + value; }
            Field.SetValue(fields, title, updatedString, fieldType);
        }

#region CxSequence Processing

        private void ProcessSequenceListFirstPass(List<CxContent> sequenceList)
        {
            sequenceList.ForEach(c => FirstPassSequenceProcessing(c));
            celtxData.sequenceProcessingFirstPassItems.ForEach(i => CheckIfRoot(i));

            List<SequenceProcessingFirstPassItem> rootItems = celtxData.sequenceProcessingFirstPassItems.Where(i => i.isRoot).ToList();

            rootItems.ForEach(i => ProcessConversation(i, sequenceList));
        }

        private void FirstPassSequenceProcessing(CxContent cxSequenceContentObject)
        {
            SequenceProcessingFirstPassItem seqItem = new SequenceProcessingFirstPassItem();
            seqItem.id = cxSequenceContentObject.attrs.id;
            seqItem.name = cxSequenceContentObject.attrs.name;
            if (cxSequenceContentObject.attrs.transitions != null && cxSequenceContentObject.attrs.transitions.Count == 1 && cxSequenceContentObject.attrs.transitions[0].id != null)
            {
                seqItem.linkedIds.Add(cxSequenceContentObject.attrs.transitions[0].id);     
            }

            // TODO: Resilience for these checks for IDs similar to what transitions have
            List<CxContent> cxBranches = cxSequenceContentObject.content[0].content.Where(c => c.type.Equals("cxbranch")).ToList();
            if (cxBranches.Count > 0)
            {
                foreach(CxContent branch in cxBranches)
                {
                    foreach(CxLinked linkedItem in branch.attrs.linked)
                    {
                        seqItem.linkedIds.Add(linkedItem.id);
                    }
                }
            }

            List<CxContent> cxJumps = cxSequenceContentObject.content[0].content.Where(c => c.type.Equals("cxjump")).ToList();
            if (cxJumps.Count > 0)
            {
                foreach (CxContent jump in cxJumps)
                {
                    seqItem.linkedIds.Add(jump.attrs.linked_jump.id);
                }
            }

            celtxData.idsWithIncomingLinks.AddRange(seqItem.linkedIds);

            celtxData.sequenceProcessingFirstPassItems.Add(seqItem);
        }

        public void CheckIfRoot(SequenceProcessingFirstPassItem i)
        {
            if (!celtxData.idsWithIncomingLinks.Contains(i.id))
            {
                i.isRoot = true;
            }
        }

        public void ProcessConversation(SequenceProcessingFirstPassItem i, List<CxContent> sequenceList) 
        {
            string sequenceId = i.id;
            CxContent cxSequenceContentObject = sequenceList.Find(c => c.attrs.id.Equals(i.id));
            DialogueEntry lastEntry = ProcessConversationRootNodeNew(cxSequenceContentObject, sequenceId);
            if (lastEntry == null) return;

            ProcessConversationBranch(sequenceId, lastEntry, i, sequenceList);
        }

        public void ProcessConversationBranch(string sourceCxId, DialogueEntry lastEntry, SequenceProcessingFirstPassItem i, List<CxContent> sequenceList)
        {

            foreach (string linkedSequenceId in i.linkedIds)
            {
                if (i.linksProcessed.Contains(linkedSequenceId))
                {
                    continue;
                }
                SequenceProcessingFirstPassItem nextItem = celtxData.sequenceProcessingFirstPassItems.Find(o => o.id.Equals(linkedSequenceId));
                if (!nextItem.isRoot)
                {
                    CxContent nextCxSequenceContentObject = sequenceList.Find(c => c.attrs.id.Equals(nextItem.id));
                    
                    DialogueEntry newLastEntry = ProcessNonRootSequenceNodeNew(nextCxSequenceContentObject, nextItem, lastEntry, sourceCxId);
                    if (newLastEntry != null)
                    {
                        ProcessConversationBranch(nextCxSequenceContentObject.attrs.id, newLastEntry, nextItem, sequenceList);
                    }
                }
                i.linksProcessed.Add(linkedSequenceId);
            }
            i.sequenceProcessingComplete = true;
        }

        private void ProcessCeltxSequenceNode(CxContent cxSequenceContentObject)
        {
            try
            {
                string sequenceId = cxSequenceContentObject.attrs.id;

                List<DialogueEntry> parentsPendingLinkToSequenceId = GetParentEntries(sequenceId);

                if (parentsPendingLinkToSequenceId.Count > 0)
                {
                    ProcessNonRootSequenceNode(cxSequenceContentObject, sequenceId, parentsPendingLinkToSequenceId);
                }
                else
                {
                    ProcessConversationRootNode(cxSequenceContentObject, sequenceId);
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, cxSequenceContentObject.attrs.id);
            }
        }

        private DialogueEntry ProcessConversationRootNodeNew(CxContent cxSequenceContentObject, string sequenceId)
        {
            try
            {
                string sequenceName = cxSequenceContentObject.attrs.name;
                string sequenceCatalogId = cxSequenceContentObject.attrs.catalog_id;

                List<CxContent> pageContents = GetSequencePageContents(cxSequenceContentObject);
                if (pageContents == null)
                {
                    LogWarning("Skipping this sequence map because the Conversation Root node's page is null or empty. Please ensure the node has 2 characters",
                        cxSequenceContentObject.attrs.id, cxSequenceContentObject.attrs.name);
                    return null;
                }

                List<CxContent> characters = pageContents.FindAll(c => c.type.Equals("cxcharacter"));

                if (characters == null || characters.Count < 2)
                {
                    LogWarning("Skipping this sequence map because the Conversation Root node has less than 2 characters. Please ensure the node has 2 characters",
                        cxSequenceContentObject.attrs.id, cxSequenceContentObject.attrs.name);
                    return null;
                }

                Conversation conversation = template.CreateConversation(template.GetNextConversationID(database), sequenceName);
                database.conversations.Add(conversation);

                conversation.ActorID = GetActorIdForCharacter(characters[0]);
                conversation.ConversantID = GetActorIdForCharacter(characters[1]);

                DialogueEntry startNode = CreateNextDialogueEntryForConversation(conversation, "START", sequenceId);
                startNode.Sequence = "None()";
                AddSetVariableScript(startNode, sequenceCatalogId);

                return startNode;

                //if (cxSequenceContentObject.attrs.transitions != null && cxSequenceContentObject.attrs.transitions.Count == 1 && cxSequenceContentObject.attrs.transitions[0].id != null)
                //{
                //    AddConditionToPendingLinks(startNode, sequenceId, cxSequenceContentObject.attrs.transitions[0].id, false);
                //}
                //else
                //{
                //    CxContent jumpData = cxSequenceContentObject.content[0].content.Find(c => c.type.Equals("cxjump"));
                //    if (jumpData == null)
                //    {
                //        LogWarning($"Can't resolve jump from {sequenceName} ({sequenceCatalogId})", sequenceCatalogId, sequenceName);
                //    }
                //    else
                //    {
                //        List<CxLinked> links = new List<CxLinked>();
                //        links.Add(new CxLinked(jumpData.attrs.linked_jump.id));
                //        CreateEntryFromBranchOrJump(jumpData, conversation, startNode, sequenceId, links, true);
                //    }
                //}
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, cxSequenceContentObject.attrs.id);
                return null;
            }
        }

        private DialogueEntry ProcessNonRootSequenceNodeNew(CxContent cxSequenceContentObject, SequenceProcessingFirstPassItem item, DialogueEntry parentsPendingLinkToSequenceId, string sourceCxId)
        {
            try
            {
                string sequenceId = item.id;

                string sequenceName = cxSequenceContentObject.attrs.name;
                string sequenceCatalogId = cxSequenceContentObject.attrs.catalog_id;

                int conversationId = -1;
                conversationId = parentsPendingLinkToSequenceId.conversationID;
                Conversation conversation = database.GetConversation(conversationId);

                DialogueEntry targetEntry = null;

                foreach (DialogueEntry entry in conversation.dialogueEntries)
                {
                    if (Field.LookupValue(entry.fields, CeltxFields.CeltxId).Equals(sequenceId))
                    {
                        targetEntry = entry;
                        break;
                    }
                }

                if (targetEntry == null)
                {
                    targetEntry = CreateNextDialogueEntryForConversation(conversation, sequenceName, sequenceId);
                    AddSetVariableScript(targetEntry, sequenceCatalogId);
                } 
                else
                {
                    string linkConditionId = GetLinkConditionId(sourceCxId, sequenceId);
                    if (parentsPendingLinkToSequenceId.outgoingLinks.Count > 0)
                    {
                        foreach (Link link in parentsPendingLinkToSequenceId.outgoingLinks)
                        {
                            string linkCxId = Field.LookupValue(conversation.GetDialogueEntry(link.destinationDialogueID).fields, CeltxFields.CeltxId);
                            string targetLinkId = Field.LookupValue(parentsPendingLinkToSequenceId.fields, CeltxFields.CeltxId) + "-" + Field.LookupValue(targetEntry.fields, CeltxFields.CeltxId);
                            if (linkCxId.Contains(targetLinkId))
                            {
                                return targetEntry;
                            }
                        }
                    }

                }

                LinkDialogueEntries(parentsPendingLinkToSequenceId, targetEntry, GetLinkConditionId(sourceCxId, sequenceId));

                // Process actual sequence node contents
                if (!item.sequenceProcessingComplete)
                {
                    targetEntry = ProcessSequenceNodeContents(targetEntry, cxSequenceContentObject);
                }
                else
                {
                    return null;
                }
                //HERE TODO RE_ADD ^^^^^

                return targetEntry;

                //if (cxSequenceContentObject.attrs.transitions != null && cxSequenceContentObject.attrs.transitions.Count == 1 && cxSequenceContentObject.attrs.transitions[0].id != null)
                //{
                //    AddConditionToPendingLinks(entry, sequenceId, cxSequenceContentObject.attrs.transitions[0].id, false);
                //}
                //else
                //{
                //    CxContent branchData = cxSequenceContentObject.content[0].content.Find(c => c.type.Equals("cxbranch"));
                //    CxContent jumpData = cxSequenceContentObject.content[0].content.Find(c => c.type.Equals("cxjump"));
                //    if (branchData != null)
                //    {
                //        CreateEntryFromBranchOrJump(branchData, conversation, entry, sequenceId, branchData.attrs.linked, false);
                //    }
                //    else if (jumpData != null)
                //    {
                //        List<CxLinked> links = new List<CxLinked>();
                //        links.Add(new CxLinked(jumpData.attrs.linked_jump.id));
                //        CreateEntryFromBranchOrJump(jumpData, conversation, entry, sequenceId, links, true);
                //    }
                //}
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, cxSequenceContentObject.attrs.id);
                return null;
            }
        }

        private List<CxContent> GetSequencePageContents(CxContent cxSequenceContentObject)
        {
            if (cxSequenceContentObject.content != null && cxSequenceContentObject.content.Count > 0)
            {
                CxContent page = cxSequenceContentObject.content[0];
                if (page.type.Equals("cxpage"))
                {
                    return page.content;
                }
            }
            return null;
        }

        private void ProcessConversationRootNode(CxContent cxSequenceContentObject, string sequenceId)
        {
            try
            {
                string sequenceName = cxSequenceContentObject.attrs.name;
                string sequenceCatalogId = cxSequenceContentObject.attrs.catalog_id;

                List<CxContent> pageContents = GetSequencePageContents(cxSequenceContentObject);
                if (pageContents == null)
                {
                    LogWarning("Skipping this sequence map because the Conversation Root node's page is null or empty. Please ensure the node has 2 characters",
                        cxSequenceContentObject.attrs.id, cxSequenceContentObject.attrs.name);
                    return;
                }

                List<CxContent> characters = pageContents.FindAll(c => c.type.Equals("cxcharacter"));

                if (characters == null || characters.Count < 2)
                {
                    LogWarning("Skipping this sequence map because the Conversation Root node has less than 2 characters. Please ensure the node has 2 characters",
                        cxSequenceContentObject.attrs.id, cxSequenceContentObject.attrs.name);
                    return;
                }

                Conversation conversation = template.CreateConversation(template.GetNextConversationID(database), sequenceName);
                database.conversations.Add(conversation);

                conversation.ActorID = GetActorIdForCharacter(characters[0]);
                conversation.ConversantID = GetActorIdForCharacter(characters[1]);

                DialogueEntry startNode = CreateNextDialogueEntryForConversation(conversation, "START", sequenceId);
                startNode.Sequence = "None()";
                AddSetVariableScript(startNode, sequenceCatalogId);

                if (cxSequenceContentObject.attrs.transitions != null && cxSequenceContentObject.attrs.transitions.Count == 1 && cxSequenceContentObject.attrs.transitions[0].id != null)
                {
                    AddConditionToPendingLinks(startNode, sequenceId, cxSequenceContentObject.attrs.transitions[0].id, false);
                } 
                else
                {
                    CxContent jumpData = cxSequenceContentObject.content[0].content.Find(c => c.type.Equals("cxjump"));
                    if (jumpData == null)
                    {
                        LogWarning($"Can't resolve jump from {sequenceName} ({sequenceCatalogId})", sequenceCatalogId, sequenceName);
                    }
                    else
                    { 
                        List<CxLinked> links = new List<CxLinked>();
                        links.Add(new CxLinked(jumpData.attrs.linked_jump.id));
                        CreateEntryFromBranchOrJump(jumpData, conversation, startNode, sequenceId, links, true);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, cxSequenceContentObject.attrs.id);
            }
        }

        private void ProcessNonRootSequenceNode(CxContent cxSequenceContentObject, string sequenceId, List<DialogueEntry> parentsPendingLinkToSequenceId)
        {
            try
            {
                string sequenceName = cxSequenceContentObject.attrs.name;
                string sequenceCatalogId = cxSequenceContentObject.attrs.catalog_id;

                int conversationId = -1;
                conversationId = parentsPendingLinkToSequenceId[0].conversationID;
                Conversation conversation = database.GetConversation(conversationId);

                DialogueEntry entry = CreateNextDialogueEntryForConversation(conversation, sequenceName, sequenceId);
                AddSetVariableScript(entry, sequenceCatalogId);
                foreach (DialogueEntry parentEntry in parentsPendingLinkToSequenceId)
                {
                    PendingLink pendingLink = celtxData.pendingLinksForDialogueEntry[parentEntry].Find(i => i.destinationSequenceId.Equals(sequenceId));
                    LinkDialogueEntries(parentEntry, entry, pendingLink.conditionId);
                    celtxData.pendingLinksForDialogueEntry[parentEntry].Remove(pendingLink);
                }

                // Process actual sequence node contents
                entry = ProcessSequenceNodeContents(entry, cxSequenceContentObject);

                if (cxSequenceContentObject.attrs.transitions != null && cxSequenceContentObject.attrs.transitions.Count == 1 && cxSequenceContentObject.attrs.transitions[0].id != null)
                {
                    AddConditionToPendingLinks(entry, sequenceId, cxSequenceContentObject.attrs.transitions[0].id, false);
                }
                else
                {
                    CxContent branchData = cxSequenceContentObject.content[0].content.Find(c => c.type.Equals("cxbranch"));
                    CxContent jumpData = cxSequenceContentObject.content[0].content.Find(c => c.type.Equals("cxjump"));
                    if (branchData != null)
                    {
                        CreateEntryFromBranchOrJump(branchData, conversation, entry, sequenceId, branchData.attrs.linked, false);
                    }
                    else if (jumpData != null)
                    {
                        List<CxLinked> links = new List<CxLinked>();
                        links.Add(new CxLinked(jumpData.attrs.linked_jump.id));
                        CreateEntryFromBranchOrJump(jumpData, conversation, entry, sequenceId, links, true);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, cxSequenceContentObject.attrs.id);
            }
        }

        private DialogueEntry ProcessSequenceNodeContents(DialogueEntry initialEntry, CxContent cxSequenceContentObject)
        {
            DialogueEntry currentEntry = initialEntry;
            Conversation conversation = database.GetConversation(currentEntry.conversationID);

            try
            {
                List<CxContent> sequencePageContentList = cxSequenceContentObject.content[0].content;
                int entryCount = 1;
                bool createNewEntry = false;


                foreach (CxContent pageContentItem in sequencePageContentList)
                {
                    if (!pageContentItem.type.Equals("cxgameplay") && 
                        !pageContentItem.type.Equals("cxcharacter") && 
                        !pageContentItem.type.Equals("cxparenthetical") && 
                        !pageContentItem.type.Equals("cxdialog"))
                    {
                        continue;
                    }
                    string combinedText = GetCombinedText(pageContentItem.content);

                    if (createNewEntry)
                    {
                        entryCount += 1;
                        currentEntry = CreateAdditionalEntryForSequence(conversation, currentEntry, initialEntry, entryCount);
                        createNewEntry = false;
                    }

                    switch (pageContentItem.type)
                    {
                        case "cxgameplay":
                            if (StringStartsWithTag(combinedText, "[SEQ]"))
                            {
                                currentEntry.Sequence = GetTextWithoutTag(combinedText);
                            }
                            break;

                        case "cxcharacter":
                            currentEntry.ActorID = GetActorIdForCharacter(pageContentItem);
                            if (currentEntry.ActorID == conversation.ActorID)
                            {
                                currentEntry.ConversantID = conversation.ConversantID;
                            } 
                            else
                            {
                                currentEntry.ConversantID = conversation.ActorID;
                            }
                            
                            break;

                        case "cxparenthetical":
                            if (StringStartsWithTag(combinedText, "[C]"))
                            {
                                currentEntry.ConversantID = database.GetActor(GetTextWithoutTag(combinedText).ToUpper()).id;
                            }
                            else if (StringStartsWithTag(combinedText, "[VO]"))
                            {
                                AppendToField(currentEntry.fields, CeltxFields.VoiceOverFile, GetTextWithoutTag(combinedText), FieldType.Files);
                            }
                            else
                            {
                                currentEntry.MenuText = combinedText;
                            }
                            break;

                        case "cxdialog":
                            currentEntry.DialogueText = GetCombinedText(pageContentItem.content);

                            createNewEntry = true;
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, cxSequenceContentObject.attrs.id);
            }

            return currentEntry;
        }

        private DialogueEntry CreateAdditionalEntryForSequence(Conversation conversation, DialogueEntry currentEntry, DialogueEntry initialEntry, int entryCount)
        {
            DialogueEntry newEntry = CreateNextDialogueEntryForConversation(conversation, initialEntry.Title + "-" + entryCount,
                Field.LookupValue(initialEntry.fields, CeltxFields.CeltxId) + "-" + entryCount);
            newEntry.ActorID = currentEntry.ActorID;
            newEntry.ConversantID = currentEntry.ConversantID;
            LinkDialogueEntries(currentEntry, newEntry, null);
            return newEntry;
        }

        private void CreateEntryFromBranchOrJump(CxContent contentData, Conversation conversation, DialogueEntry sourceEntry, string sequenceId, List<CxLinked> links, bool isJump)
        {
            try
            {

                if (contentData != null)
                {
                    string cxId = contentData.attrs.id;
                    string name = contentData.attrs.name;
                    string catalogId = contentData.attrs.catalog_id;
                    DialogueEntry newEntry = CreateNextDialogueEntryForConversation(conversation, name, cxId, true);
                    AddSetVariableScript(newEntry, catalogId);
                    LinkDialogueEntries(sourceEntry, newEntry, GetLinkConditionId(sequenceId, cxId));

                    foreach (CxLinked link in links)
                    {
                        AddConditionToPendingLinks(newEntry, cxId, link.id, isJump);
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, sequenceId);
            }
}

        private void AddConditionToPendingLinks(DialogueEntry sourceEntry, string sourceCxId, string targetCxId, bool isJumpLink)
        {
            string conditionId = GetLinkConditionId(sourceCxId, targetCxId);
            PendingLink pendingNonJumpLink = new PendingLink(targetCxId, conditionId, isJumpLink);
            AddToPendingLinks(sourceEntry, pendingNonJumpLink);
        }

        private void AddSetVariableScript(DialogueEntry entry, string sequenceCatalogId)
        {
            try
            {
                if (celtxData.customVarListLookupByCxSequenceId.ContainsKey(sequenceCatalogId))
                {
                    List<CxCustomVar> cxCustomVars = celtxData.customVarListLookupByCxSequenceId[sequenceCatalogId];
                    StringBuilder luaScript = new StringBuilder();
                    foreach (CxCustomVar cxCustomVar in cxCustomVars)
                    {
                        if (luaScript.Length != 0) { luaScript.Append("; "); }
                        Variable var = database.GetVariable(celtxData.variableLookupByCeltxId[cxCustomVar.id]);
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Variable");
                        stringBuilder.Append("[\"" + var.Name + "\"]");
                        stringBuilder.Append(" = ");

                        if (var.Type == FieldType.Boolean)
                        {
                            if (!cxCustomVar.boolVal.ToString().Equals(var.InitialValue)) { stringBuilder.Append(cxCustomVar.boolVal.ToString().ToLower()); }
                            else { stringBuilder.Append(var.InitialValue); }
                        }
                        else if (var.Type == FieldType.Number)
                        {
                            if (!cxCustomVar.longVal.ToString().Equals(var.InitialValue)) { stringBuilder.Append(cxCustomVar.longVal.ToString()); }
                            else { stringBuilder.Append(var.InitialValue); }
                        }
                        else if (var.Type == FieldType.Text)
                        {
                            if (cxCustomVar.type != null && cxCustomVar.type.Equals("radio"))
                            {
                                stringBuilder.Append(cxCustomVar.config.options[(int)cxCustomVar.longVal].strVal);
                            }
                            else
                            {
                                if (cxCustomVar.strVal != null && !cxCustomVar.strVal.Equals(var.InitialValue)) { stringBuilder.Append("\"" + cxCustomVar.strVal + "\""); }
                                else { stringBuilder.Append("\"" + var.InitialValue + "\""); }
                            }
                        }
                        luaScript.Append(stringBuilder.ToString());
                    }
                    entry.userScript = luaScript.ToString();
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, "SetVar[" + sequenceCatalogId + "]");
            }
        }

        private List<DialogueEntry> GetParentEntries(string sequenceId)
        {
            List<DialogueEntry> parentEntries = new List<DialogueEntry>();
            foreach (DialogueEntry entry in celtxData.pendingLinksForDialogueEntry.Keys)
            {
                foreach (PendingLink pendingLink in celtxData.pendingLinksForDialogueEntry[entry])
                {
                    if (pendingLink.destinationSequenceId.Equals(sequenceId) && !pendingLink.isJumpLink)
                    {
                        parentEntries.Add(entry);
                    }
                }
            }
            return parentEntries;
        }



        private int GetActorIdForCharacter(CxContent characterContentItem)
        {
            return celtxData.actorIdLookupByCxCharacterCatalogId[characterContentItem.attrs.catalog_id];
        }
         
        private DialogueEntry CreateNextDialogueEntryForConversation(Conversation conversation, string title, string celtxId, bool isGroup = false)
        {
            try
            {
                DialogueEntry dialogueEntry = template.CreateDialogueEntry(template.GetNextDialogueEntryID(conversation), conversation.id, title);
                conversation.dialogueEntries.Add(dialogueEntry);
                dialogueEntry.isGroup = isGroup;
                dialogueEntry.ActorID = conversation.ActorID;
                dialogueEntry.ConversantID = conversation.ConversantID;
                if (celtxId != null)
                {
                    Field.SetValue(dialogueEntry.fields, CeltxFields.CeltxId, celtxId);
                    celtxData.dialogueEntryLookupByCeltxId.Add(celtxId, dialogueEntry);
                }
                return dialogueEntry;
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, celtxId, title);
            }
            return null;
        }
#endregion

#region Linking
        private void AddToPendingLinks(DialogueEntry parentEntry, PendingLink pendingLink)
        {
            if (celtxData.pendingLinksForDialogueEntry.ContainsKey(parentEntry))
            {
                celtxData.pendingLinksForDialogueEntry[parentEntry].Add(pendingLink);
            }
            else
            {
                List<PendingLink> pendingNonJumpLinks = new List<PendingLink>();
                pendingNonJumpLinks.Add(pendingLink);
                celtxData.pendingLinksForDialogueEntry.Add(parentEntry, pendingNonJumpLinks);
            }
        }

        private string GetLinkConditionId(string sourceCeltxId, string destinationCeltxId)
        {
            try { 
                foreach (CeltxCondition condition in celtxData.conditions)
                {
                    foreach (CxOnObj onObj in condition.links)
                    {
                        if (onObj.from.Equals(sourceCeltxId) && onObj.to.Equals(destinationCeltxId))
                        {
                            return condition.id;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, sourceCeltxId + "-" + destinationCeltxId);
            }
            return null;
        }

        private void LinkDialogueEntries(DialogueEntry source, DialogueEntry destination, string conditionId)
        {
            try
            {
                if (conditionId == null)
                {
                    source.outgoingLinks.Add(new Link(source.conversationID, source.id, destination.conversationID, destination.id));
                }
                else
                {
                    CeltxCondition condition = celtxData.conditions.Find(c => c.id.Equals(conditionId));
                    DialogueEntry entryToLinkToCondition = source;
                    if (condition.delay)
                    {
                        DialogueEntry delayEntry = CreateNextDialogueEntryForConversation(database.GetConversation(source.conversationID),
                            "[D]-" + condition.name, 
                            "D-" + Field.LookupValue(source.fields, CeltxFields.CeltxId) + "-" + Field.LookupValue(destination.fields, CeltxFields.CeltxId), true);
                        source.outgoingLinks.Add(new Link(source.conversationID, source.id, delayEntry.conversationID, delayEntry.id));
                        entryToLinkToCondition = delayEntry;
                    }

                    DialogueEntry conditionEntry = CreateNextDialogueEntryForConversation(database.GetConversation(source.conversationID),
                        "[COND]" + condition.name, 
                        "C-" + Field.LookupValue(source.fields, CeltxFields.CeltxId) + "-" + Field.LookupValue(destination.fields, CeltxFields.CeltxId), true);
                    conditionEntry.conditionsString = condition.luaConditionString;
                    entryToLinkToCondition.outgoingLinks.Add(new Link(entryToLinkToCondition.conversationID, entryToLinkToCondition.id, conditionEntry.conversationID, conditionEntry.id));
                    conditionEntry.outgoingLinks.Add(new Link(conditionEntry.conversationID, conditionEntry.id, destination.conversationID, destination.id));
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, source.Title + "-" + destination.Title + "-" + conditionId);
            }
}

        private void ResolvePendingLinks()
        {
            foreach (DialogueEntry parentEntry in celtxData.pendingLinksForDialogueEntry.Keys)
            {
                if (parentEntry == null)
                {
                    continue;
                }
                else if (!celtxData.pendingLinksForDialogueEntry.ContainsKey(parentEntry))
                {
                    LogWarning($"Can't resolve link from {parentEntry} because a link from the entry wasn't added to the pending list.", parentEntry.Title, "");
                }
                else
                {
                    foreach (PendingLink pending in celtxData.pendingLinksForDialogueEntry[parentEntry])
                    {
                        if (pending.destinationSequenceId == null)
                        {
                            continue;
                        }
                        else if (!celtxData.dialogueEntryLookupByCeltxId.ContainsKey(pending.destinationSequenceId))
                        {
                            LogWarning($"Can't resolve link to sequence with Celtx Id {pending.destinationSequenceId} because the sequence wasn't added to the dialogue database.", pending.destinationSequenceId, "");
                        }
                        else
                        {
                            DialogueEntry destEntry = celtxData.dialogueEntryLookupByCeltxId[pending.destinationSequenceId];
                            LinkDialogueEntries(parentEntry, destEntry, pending.conditionId);
                        }
                    }
                }
            }
            
            celtxData.pendingLinksForDialogueEntry.Clear();
        }

#endregion

#region Text Processing
        private bool StringStartsWithTag(string stringToCheck, string targetTag)
        {
            if (stringToCheck == null || targetTag == null){ return false; }

            return stringToCheck.ToUpper().StartsWith(targetTag);
        }

        private string GetTextWithoutTag(string text)
        {
            if (text == null) { return ""; }

            return System.Text.RegularExpressions.Regex.Split(text, "(\\[.*\\])")[2].Trim();            
        }

        private string GetCombinedText(List<CxContent> contentList)
        {
            if (contentList == null) { return ""; }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (CxContent content in contentList)
            {
                if (content.type.Equals("text"))
                {
                    stringBuilder.Append(GetTextWithMarks(content));
                }
            }
            return stringBuilder.ToString();
        }

        private string GetTextWithMarks(CxContent textContent)
        {
            try
            {
                string text = textContent.text;
                if (textContent.marks != null && textContent.marks.Count > 0)
                {
                    string prependString = "";
                    string appendString = "";
                    foreach (CxContent mark in textContent.marks)
                    {
                        switch (mark.type)
                        {
                            case "strong":
                                prependString = prependString + "<b>";
                                appendString = "</b>" + appendString;
                                break;

                            case "em":
                                prependString = prependString + "<i>";
                                appendString = "</i>" + appendString;
                                break;

                            case "underline":
                                prependString = prependString + "<u>";
                                appendString = "</u>" + appendString;
                                break;
                        }
                    }
                    return prependString + text + appendString;
                }
                else
                {
                    return text;
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, textContent.attrs.id);
                return "";
            }
        }
#endregion

#region Catalog Data Processing

        private void ConvertCharacterCatalogEntryToDSActor(CxAttrs catalogItemAttrs)
        {
            try
            {
                Actor actor = database.GetActor(catalogItemAttrs.title);
                if (actor == null)
                {
                    int actorID = template.GetNextActorID(database);
                    actor = template.CreateActor(actorID, catalogItemAttrs.title, IsPlayerCharacter(catalogItemAttrs));
                    database.actors.Add(actor);

                    celtxData.actorIdLookupByCxCharacterCatalogId.Add(catalogItemAttrs.id, actorID);
                }

                AppendToField(actor.fields, CeltxFields.CatalogId, catalogItemAttrs.id, FieldType.Text);
                AppendToField(actor.fields, CeltxFields.Description, catalogItemAttrs.item_data.description, FieldType.Text);
                AppendToField(actor.fields, CeltxFields.Pictures, getPictureString(catalogItemAttrs), FieldType.Files);
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, catalogItemAttrs.id, catalogItemAttrs.title);
            }
        }

        private bool IsPlayerCharacter(CxAttrs characterAttrs)
        {
            try
            {
                if (characterAttrs.item_data == null)
                {
                    LogWarning(GetNullDataString("attrs.item_data") +
                        "Cannot determine if character is a player. Please ensure Character Type is set in the catalog", characterAttrs.id, characterAttrs.title);
                    return false;
                }

                if (characterAttrs.item_data.character_type == null)
                {
                    LogWarning(GetNullDataString("attrs.item_data.character_type") +
                        "Defaulting to non-player Actor. Please ensure Character Type is set in the catalog", characterAttrs.id, characterAttrs.title);
                    return false;
                }

                return characterAttrs.item_data.character_type.Equals("pc");
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, characterAttrs.id, characterAttrs.title);
            }
            return false;
        }

        private string getPictureString(CxAttrs catalogItemAttrs)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("[");
                if (catalogItemAttrs.item_data.media != null && catalogItemAttrs.item_data.media.Count > 0)
                {
                    foreach (CxMedia media in catalogItemAttrs.item_data.media)
                    {
                        stringBuilder.Append(media.name);

                        if (media.Equals(catalogItemAttrs.item_data.media.Last()))
                        {
                            stringBuilder.Append("]");
                        }
                        else
                        {
                            stringBuilder.Append(";");
                        }
                    }
                    return stringBuilder.ToString();
                }
                else
                {
                    return "[]";
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, catalogItemAttrs.id, catalogItemAttrs.title);
            }
            return "[]";
        }

        private void ConvertLocationCatalogEntryToDSLocation(CxAttrs catalogItemAttrs)
        {
            try
            {
                Location location = database.GetLocation(catalogItemAttrs.title);

                if (location == null)
                {
                    int locationId = template.GetNextLocationID(database);
                    location = template.CreateLocation(locationId, catalogItemAttrs.title);
                    database.locations.Add(location);
                }

                AppendToField(location.fields, CeltxFields.CatalogId, catalogItemAttrs.id, FieldType.Text);
                AppendToField(location.fields, CeltxFields.Description, catalogItemAttrs.item_data.description, FieldType.Text);
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, catalogItemAttrs.id, catalogItemAttrs.title);
            }
        }

        private void ConvertItemCatalogEntryToDSItem(CxAttrs catalogItemAttrs)
        {
            try
            {
                Item item = database.GetItem(catalogItemAttrs.title);

                if (item == null)
                {
                    int itemId = template.GetNextItemID(database);
                    item = template.CreateItem(itemId, catalogItemAttrs.title);
                    database.items.Add(item);
                }

                AppendToField(item.fields, CeltxFields.CatalogId, catalogItemAttrs.id, FieldType.Text);
                AppendToField(item.fields, CeltxFields.Description, catalogItemAttrs.item_data.description, FieldType.Text);

                AppendToField(item.fields, CeltxFields.ItemType, catalogItemAttrs.item_data.item_type, FieldType.Text);
                AppendToField(item.fields, CeltxFields.ItemProperties, catalogItemAttrs.item_data.item_properties, FieldType.Text);
                AppendToField(item.fields, CeltxFields.ItemAvailability, catalogItemAttrs.item_data.item_availability, FieldType.Text);
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, catalogItemAttrs.id, catalogItemAttrs.title);
            }
        }

        private void ExtractCustomVarsList(CxAttrs catalogItemAttrs)
        {
            try
            {
                string catalogId = catalogItemAttrs.id;
                List<CxCustomVar> customVars = catalogItemAttrs.custom_vars;

                if (customVars != null && customVars.Count > 0)
                {
                    celtxData.customVarListLookupByCxSequenceId.Add(catalogId, customVars);
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, catalogItemAttrs.id, catalogItemAttrs.title);
            }
        }
#endregion

#region Variables and Conditions

        private void ConvertCeltxVariableToDSVariable(CxVariable variableObject)
        {
            try
            {
                FieldType fieldType = FieldType.Text;
                string variableDefaultValue = "";
                List<Field> extraFields = new List<Field>();

                switch (variableObject.type)
                {
                    case "boolean":
                        variableDefaultValue = variableObject.config.defaultBool.ToString();
                        fieldType = FieldType.Boolean;
                        break;

                    case "number":
                    case "range":
                        variableDefaultValue = variableObject.config.defaultNum.ToString();
                        fieldType = FieldType.Number;
                        break;

                    case "date":
                    case "text":
                    case "textarea":
                    case "time":
                        variableDefaultValue = variableObject.config.defaultString;
                        fieldType = FieldType.Text;
                        break;

                    case "radio":
                        variableDefaultValue = variableObject.config.options[(int) variableObject.config.defaultNum].strVal;
                        fieldType = FieldType.Text;
                        break;
                }

                int variableId = template.GetNextVariableID(database);
                Variable variable = template.CreateVariable(variableId, variableObject.name, variableDefaultValue, fieldType);
                database.variables.Add(variable);

                AppendToField(variable.fields, CeltxFields.Description, variableObject.desc, FieldType.Text);
                celtxData.variableLookupByCeltxId.Add(variableObject.id, variableObject.name);
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, variableObject.id, variableObject.name);
            }
        }

        private void ExtractCeltxCondition(CxAttrs conditionItemAttrs)
        {
            try { 
                CeltxCondition condition = new CeltxCondition();
                condition.id = conditionItemAttrs.id;
                condition.catalogId = conditionItemAttrs.catalog_id;
                condition.name = conditionItemAttrs.name;
                if (conditionItemAttrs.desc != null)
                {
                    condition.delay = conditionItemAttrs.desc.Contains("[D]");
                }
                if (condition.delay) condition.description = conditionItemAttrs.desc.Substring(0, "[D]".Length);

                condition.links = conditionItemAttrs.on;
                condition.literals = conditionItemAttrs.literals;
                SetFollowingOperators(condition, conditionItemAttrs.clause);
                celtxData.conditions.Add(condition);
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, conditionItemAttrs.id, conditionItemAttrs.title);
            }
        }

        private void SetFollowingOperators(CeltxCondition condition, string clause)
        {
            try
            {
                List<string> subSections = new List<string>();
                List<string> orSplit = new List<string>();
                subSections.AddRange(System.Text.RegularExpressions.Regex.Split(clause, "(\\+|\\.)"));

                if (subSections.ElementAt(subSections.Count - 1).Equals("")) { subSections.RemoveAt(subSections.Count - 1); }

                for (int i = 0; i < subSections.Count - 1; i += 2)
                {
                    CxLiteral literal = condition.literals.Find(l => l.lit_name.Equals(subSections.ElementAt(i)));
                    literal.followingOp = subSections.ElementAt(i + 1);
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, condition.id, condition.name);
            }
}

        private void GenerateLuaCondition(CeltxCondition condition)
        {
            try
            {
                if (condition.literals == null)
                {
                    LogMessage(LogLevel.W,
                            "Condition has no literals(literals list is null). Lua script generation will be skipped for this condition. Please add literals to the condition in CeltX", condition.id);
                    return;
                }

                StringBuilder conditionString = new StringBuilder();
                string followingOp = null;
                foreach (CxLiteral literal in condition.literals)
                {
                    if (followingOp != null) { conditionString.Append(" " + followingOp + " "); }

                    if (literal.type.Equals("variable"))
                    {
                        conditionString.Append("Variable");
                        conditionString.Append("[\"" + literal.name + "\"]");
                        conditionString.Append(" " + GetLuaComparisonOperator(literal.comparisonOperator) + " ");
                        string comparisonVal;
                        if (literal.varType != null && literal.varType.Equals("radio"))
                        {
                            comparisonVal = literal.config.options[(int)literal.comparisonValue.longVal].strVal;
                        }
                        else
                        {
                            comparisonVal = literal.comparisonValue.strVal;
                        }

                        if (SurroundComparisonValueInQuotes(literal))
                        {
                            conditionString.Append("\"" + comparisonVal + "\"");
                        } 
                        else
                        {
                            conditionString.Append(comparisonVal);
                        }
                    }
                    else if (literal.type.Equals("condition"))
                    {
                        conditionString.Append("(");
                        conditionString.Append(GetNestedConditionString(literal.conditionId));
                        conditionString.Append(") == ");
                        conditionString.Append(literal.comparisonValue.strVal);
                    }
                    else
                    {
                        LogMessage(LogLevel.W,
                            "Unsupported literal type(" + literal.type + ") in condition. Only variable and condition literal types are supported", literal.id);
                        continue;
                    }

                    if (literal.followingOp.Equals(".")) { followingOp = "and"; }
                    else if (literal.followingOp.Equals("+")) { followingOp = "or"; }
                    else
                    {
                        LogMessage(LogLevel.W,
                            "Unsupported following operator(" + literal.followingOp + ") in condition. Only \".\"(AND) and \"+\"(OR) are supported", literal.id);
                        continue;
                    }
                }
                condition.luaConditionString = conditionString.ToString();
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, condition.id, condition.name);
            }
        }

        private bool SurroundComparisonValueInQuotes(CxLiteral literal)
        {
            try { 
                string variableId = literal.id;
                Variable var = database.GetVariable(celtxData.variableLookupByCeltxId[variableId]);
                FieldType varType = var.Type;
                if (varType == FieldType.Text)
                {
                    return true;
                } 
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, literal.id);
            }
            return false;
        }

        private string GetNestedConditionString(string conditionId)
        {
            try
            {
                CeltxCondition condition = celtxData.conditions.Find(c => c.id.Equals(conditionId));
                if (condition.luaConditionString == null)
                {
                    GenerateLuaCondition(condition);
                }

                return condition.luaConditionString;
            }
            catch (System.Exception e)
            {
                LogError(MethodBase.GetCurrentMethod(), e, conditionId);
            }
            return null;            
        }

        private string GetLuaComparisonOperator(string literalOp)
        {
            switch (literalOp)
            {
                case "<":
                case "<=":
                case ">":
                case ">=":
                    return literalOp;

                case "=":
                    return "==";

                case "!=":
                    return "~=";

                case "exists":
                    LogMessage(LogLevel.W,
                        "Literal operator(" + literalOp + ") not supported. Defaulting to \"==\"");
                    return "==";

                case "does not exist":
                    LogMessage(LogLevel.W,
                        "Literal operator(" + literalOp + ") not supported. Defaulting to \"~=\"");
                    return "~=";

                default:
                    LogMessage(LogLevel.W,
                        "Unknown comparison operator(" + literalOp + ") in literal. Defaulting to \"==\"");
                    return "==";
            }
        }

#endregion

#region Check Sequence Syntax

        /// <summary>
        /// Check syntax of all sequences in all dialogue entries in database.
        /// Log warning for any entries with syntax errors.
        /// </summary>
        private void CheckSequenceSyntax()
        {
            if (database == null) return;
            var parser = new SequenceParser();
            foreach (Conversation conversation in database.conversations)
            {
                foreach (DialogueEntry entry in conversation.dialogueEntries)
                {
                    var sequence = entry.Sequence;
                    if (string.IsNullOrEmpty(sequence)) continue;
                    var result = parser.Parse(sequence);
                    if (result == null || result.Count == 0)
                    {
                        var text = entry.Title;
                        if (string.IsNullOrEmpty(text)) text = entry.subtitleText;
                        LogWarning("Dialogue entry " + conversation.id + ":" + entry.id + 
                            " in conversation '" + conversation.Title + "' has syntax error in [SEQ] Sequence: " + sequence,
                            Field.LookupValue(entry.fields, "Celtx ID"), entry.Title);
                    }
                }
            }
        }

#endregion

#region Logging

        private string GetNullDataString(string nullFieldName)
        {
            return nullFieldName + " is null - ";
        }

        private string GetErrorString(MethodBase methodName, System.Exception ex)
        {
            return "Error in " + methodName + " : " + ex.ToString();
        }

        private void LogError(MethodBase methodName, System.Exception ex, string celtxId = null, string name = null)
        {
            string messageCore = GetErrorString(methodName, ex);
            LogMessage(LogLevel.E, messageCore, celtxId, name);
        }

        private void LogWarning(string messageCore, string celtxId, string name)
        {
            LogMessage(LogLevel.W, messageCore, celtxId, name);
        }

        private void LogMessage(LogLevel logLevel, string messageCore, string celtxId = null, string name = null)
        {
            StringBuilder logMessage = new StringBuilder();
            if (celtxId != null) { logMessage.Append("CeltX Object : " + celtxId + "(" + name + ")"); }
            logMessage.Append(" | " + messageCore);
            if (logLevel == LogLevel.W)
            {
                Debug.LogWarning(logMessage.ToString());
            } 
            else if (logLevel == LogLevel.E)
            {
                Debug.LogError(logMessage.ToString());
            }
            else
            {
                Debug.Log(logMessage.ToString());
            }
        }

        enum  LogLevel
        {
            W, E, I
        }

#endregion
    }
}
#endif
