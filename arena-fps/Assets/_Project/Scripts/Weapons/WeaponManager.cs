using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Sits on the player. Manages all 4 weapon slots, switching, firing, and drops on death.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private Transform weaponHolder; // child of CameraHolder
    [SerializeField] private Camera playerCamera;

    private WeaponBase[] _slots = new WeaponBase[4]; // 0=MG, 1=SG, 2=Rail, 3=Rocket
    private int _currentSlot = 0;

    private void Start()
    {
        // Always start with machine gun
        GiveWeapon(WeaponPickup.WeaponType.MachineGun);
        EquipSlot(0);
    }

    private void Update()
    {
        // Fire
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            _slots[_currentSlot]?.TryFire();

        // Scroll wheel switch
        float scroll = Mouse.current?.scroll.ReadValue().y ?? 0f;
        if (scroll > 0f) CycleWeapon(1);
        if (scroll < 0f) CycleWeapon(-1);
    }

    // ── Input callbacks (number keys via PlayerInput SendMessages) ──

    public void OnWeapon1(InputValue _) => EquipSlot(0);
    public void OnWeapon2(InputValue _) => EquipSlot(1);
    public void OnWeapon3(InputValue _) => EquipSlot(2);
    public void OnWeapon4(InputValue _) => EquipSlot(3);

    // ── Pickup ──────────────────────────────────────────────────────

    /// <summary>Returns true if weapon was picked up (new or ammo added).</summary>
    public bool PickupWeapon(WeaponPickup.WeaponType type)
    {
        int slot = (int)type;

        if (_slots[slot] != null)
        {
            // Already have it — just add ammo
            _slots[slot].AddAmmo(_slots[slot].maxAmmo / 2);
            return true;
        }

        GiveWeapon(type);
        return true;
    }

    // ── Switching ────────────────────────────────────────────────────

    private void CycleWeapon(int dir)
    {
        int next = _currentSlot;
        for (int i = 0; i < 4; i++)
        {
            next = (next + dir + 4) % 4;
            if (_slots[next] != null) { EquipSlot(next); return; }
        }
    }

    private void EquipSlot(int slot)
    {
        if (_slots[slot] == null) return;

        for (int i = 0; i < 4; i++)
            if (_slots[i] != null) _slots[i].gameObject.SetActive(i == slot);

        _currentSlot = slot;
    }

    // ── Drop on death ────────────────────────────────────────────────

    public void DropAll()
    {
        for (int i = 0; i < 4; i++)
        {
            if (_slots[i] == null) continue;

            // Spawn a pickup at player's feet
            var dropGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dropGo.name = _slots[i].weaponName + "_Drop";
            dropGo.transform.position = transform.position + Vector3.up * 0.5f;
            dropGo.transform.localScale = Vector3.one * 0.4f;

            var pickup = dropGo.AddComponent<WeaponPickup>();
            pickup.Init((WeaponPickup.WeaponType)i, 30f);

            var rb = dropGo.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse);

            Destroy(_slots[i].gameObject);
            _slots[i] = null;
        }
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void GiveWeapon(WeaponPickup.WeaponType type)
    {
        int slot = (int)type;
        if (_slots[slot] != null) return;

        var go = new GameObject(type.ToString());
        go.transform.SetParent(weaponHolder != null ? weaponHolder : transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        WeaponBase weapon = type switch
        {
            WeaponPickup.WeaponType.MachineGun     => go.AddComponent<MachineGun>(),
            WeaponPickup.WeaponType.Shotgun        => go.AddComponent<Shotgun>(),
            WeaponPickup.WeaponType.Railgun        => go.AddComponent<Railgun>(),
            WeaponPickup.WeaponType.RocketLauncher => go.AddComponent<RocketLauncher>(),
            _                                      => go.AddComponent<MachineGun>()
        };

        weapon.playerCamera = playerCamera != null
            ? playerCamera
            : Camera.main;

        _slots[slot] = weapon;
        go.SetActive(false);
    }

    public WeaponBase CurrentWeapon => _slots[_currentSlot];
}
