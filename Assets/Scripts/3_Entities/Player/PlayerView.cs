using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] Animator myAnim;
    public Animator MyAnimator => myAnim;
    [SerializeField] PlayerAnimationNames animation_names;

    //[SerializeField] ParticleSystem slash;
    [SerializeField] ParticleSystem hit;

    //[SerializeField] AudioClip clip_sword_Whoosh;
    //[SerializeField] AudioClip clip_TakeDamage;
    //[SerializeField] AudioClip clip_Die;
    //[SerializeField] AudioClip[] clips_Walk;

    [SerializeField] TrailRenderer dashtrail;

    private void Start()
    {
        //AudioManager.instance.GetSoundPool(clip_sword_Whoosh.name, AudioManager.SoundDimesion.TwoD, AudioGroups.GAME_FX, clip_sword_Whoosh);
        //AudioManager.instance.GetSoundPool(clip_TakeDamage.name, AudioManager.SoundDimesion.TwoD, AudioGroups.GAME_FX, clip_TakeDamage);
        //AudioManager.instance.GetSoundPool(clip_Die.name, AudioManager.SoundDimesion.TwoD, AudioGroups.GAME_FX, clip_Die);
        //foreach (var c in clips_Walk) AudioManager.instance.GetSoundPool(c.name, AudioManager.SoundDimesion.TwoD, AudioGroups.GAME_FX, c);
        ParticlesManager.Instance.GetParticlePool(hit.name, hit);
    }

    public void ANIM_Velocity(float Velocity)
    {
        myAnim.SetFloat(animation_names.PARAM_VELOCITY, Velocity);
    }

    public void ANIM_Dash(bool value)
    {
        myAnim.SetBool(animation_names.PARAM_DASHING, value);
    }

    public void ANIM_DoAttackAttack()
    {
        myAnim.SetTrigger(animation_names.PARAM_ATTACK);
    }
    public void ANIM_IsMoving(bool value)
    {
        myAnim.SetBool(animation_names.PARAM_Moving, value);
    }
    public void ANIM_IsJumping(bool value)
    {
        myAnim.SetBool(animation_names.PARAM_JUMPING, value);
    }
    public void ANIM_SetSpeed(float value)
    {
        myAnim.SetFloat(animation_names.PARAM_ATTACK, value);
    }

    public void ANIM_SetDirValue(float x, float y)
    {
        myAnim.SetFloat(animation_names.PARAM_FORWARD, x);
        myAnim.SetFloat(animation_names.PARAM_RIGHT, y);
    }

    public void Play_Hit(Vector3 position)
    {
        hit.Stop();
        hit.Play();
        TimeStop.Stop();
        CinemachineCameraShake.Shake(CinemachineCameraShake.CameraShakeType.hit_enemy);
        ParticlesManager.Instance.PlayParticle(hit.name, position);
        SoundFX.Play_Sarten_Hit();
    }

    public void BeginDash()
    {
        if (dashtrail) dashtrail.emitting = true;
        myAnim.SetBool(animation_names.PARAM_DASHING, true);

    }
    public void EndDash()
    {
        if (dashtrail) dashtrail.emitting = false;
        myAnim.SetBool(animation_names.PARAM_DASHING, false);
    }
    public void ANIM_AttackInOpenWindow(bool value) { myAnim?.SetBool("isAttackingInOpenWindow", value); }
    public void Play_Slash() { /*slash.Stop(); slash.Play(); Play_Clip_SwordWhoosh();*/ }
    public void BeginMove() { /*myAnim?.SetBool(names.NAME_ON_MOVE, true);*/ }
    public void StopMove() { /*myAnim?.SetBool(names.NAME_ON_MOVE, false);*/ }
    public void Run(bool run) { /*myAnim?.SetBool(names.NAME_RUN, run);*/ }
    public void Jump() { /*myAnim?.SetTrigger(names.NAME_JUMP); */}
    public void IsGrounded(bool isGrounded) { /*myAnim?.SetBool(names.NAME_IS_GROUNDED, isGrounded);*/ }

    public void Play_Clip_SwordWhoosh() { /*AudioManager.instance.PlaySound(clip_sword_Whoosh.name);*/ }
    public void Play_Clip_TakeDamage() { /*AudioManager.instance.PlaySound(clip_TakeDamage.name);*/ }
    public void Play_Clip_Die() { /*AudioManager.instance.PlaySound(clip_Die.name);*/ }
    public void Play_Clip_Walk() { /**AudioManager.instance.PlaySound(clips_Walk[Random.Range(0, clips_Walk.Length - 1)].name);*/ }
}

[System.Serializable]
public class PlayerAnimationNames
{
    public string NAME_RUN = "run";
    public string NAME_JUMP = "jump";
    public string NAME_IS_GROUNDED = "isGrounded";
    public string PARAM_ATTACK = "doAttack";
    public string PARAM_SPEED = "speed";
    public string PARAM_FORWARD = "forward_value";
    public string PARAM_RIGHT = "right_value";
    public string PARAM_Moving = "isMoving";
    public string PARAM_JUMPING = "isJumping";
    public string PARAM_DASHING = "isDashing";
    public string PARAM_VELOCITY = "Velocity";
}


