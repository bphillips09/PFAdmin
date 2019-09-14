using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.AdminModels;

public class PolicyID : MonoBehaviour
{
    [HideInInspector] public PolicyAction Action;
    [HideInInspector] public ApiCondition ApiConditions;
    [HideInInspector] public string Comment;
    [HideInInspector] public EffectType Effect;
    [HideInInspector] public string Principal;
    [HideInInspector] public string Resource;
    [SerializeField] private Text commentText;
    [SerializeField] private Text resourceText;
    [SerializeField] private Text actionText;
    [SerializeField] private Text principalText;
    [SerializeField] private Text effectText;
    public Button upButton;
    public Button downButton;
    private PermissionStatement thisPermission;

    public void SetPolicy(string policyAction, ApiCondition condition, string policyComment,
                          EffectType policyEffect, string policyPrincipal, string policyResource) {
        
        thisPermission = new PermissionStatement{
            Action = policyAction,
            ApiConditions = condition,
            Comment = policyComment,
            Effect = policyEffect,
            Principal = policyPrincipal,
            Resource = policyResource
        };

        Action = PlayerController.GetPolicyAction(policyAction);
        ApiConditions = condition;
        Comment = policyComment;
        Effect = policyEffect;
        Principal = policyPrincipal;
        Resource = policyResource;

        commentText.text = $"<i>{policyComment}</i>";
        resourceText.text = $"<b>Resource</b>\n\"{policyResource}\"";
        actionText.text = "<b>Action</b>\n" + policyAction;
        principalText.text = "<b>Principal</b>\n" + policyPrincipal;
        effectText.text = "<b>Effect</b>\n" + policyEffect.ToString();
    }

    public PermissionStatement GetPermissionStatement() {
        return thisPermission;
    }
}

public enum PolicyAction {ALL, Read, Write, Accept, POST, GET}